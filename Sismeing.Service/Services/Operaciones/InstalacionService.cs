using Microsoft.EntityFrameworkCore;
using Sismeing.Domain.Entities.Operaciones;
using Sismeing.Infrestructura.Persistence;
using Sismeing.Service.Interfaces.Comunes;
using Sismeing.Service.Interfaces.Operaciones;
using Sismeing.Service.Services.Comunes;

namespace Sismeing.Service.Services.Operaciones
{
    public class InstalacionService : IInstalacionService
    {
        private readonly SupaBaseDBcontext _context;
        private readonly IAuditoriaService _auditoriaService;
        private readonly INotificacionService _notificacionService;
        private readonly IUsuarioContext _usuarioContext;

        public InstalacionService(SupaBaseDBcontext context, IAuditoriaService auditoriaService, INotificacionService notificacionService, IUsuarioContext usuarioContext)
        {
            _context = context;
            _auditoriaService = auditoriaService;
            _notificacionService = notificacionService;
            _usuarioContext = usuarioContext;
        }

        public async Task<IEnumerable<Instalacion>> GetAllAsync()
        {
            var query = _context.Instalaciones
                .Include(i => i.Equipo).ThenInclude(e => e.Marca)
                .Include(i => i.Equipo).ThenInclude(e => e.Proyecto)
                .Include(i => i.Area).ThenInclude(a => a.Empresa)
                .Include(i => i.Tecnico)
                .Include(i => i.Colaboradores).ThenInclude(c => c.Usuario)
                .Include(i => i.Estado)
                .AsQueryable();

            // El Cliente solo ve las instalaciones de su empresa (por el area).
            if (_usuarioContext.EsCliente && _usuarioContext.EmpresaId is int empId)
                query = query.Where(i => i.Area != null && i.Area.EmpresaId == empId);

            return await query.ToListAsync();
        }

        public async Task<Instalacion?> GetByIdAsync(int id)
        {
            return await _context.Instalaciones
                .Include(i => i.Equipo).ThenInclude(e => e.Marca)
                .Include(i => i.Equipo).ThenInclude(e => e.Proyecto)
                .Include(i => i.Area).ThenInclude(a => a.Empresa)
                .Include(i => i.Tecnico)
                .Include(i => i.Colaboradores).ThenInclude(c => c.Usuario)
                .Include(i => i.Estado)
                .FirstOrDefaultAsync(i => i.Id == id);
        }

        private static void NormalizarFechas(Instalacion item)
        {
            item.FechaInicio = EntityUpdateHelper.AsegurarUtc(item.FechaInicio);
            item.FechaFin = EntityUpdateHelper.AsegurarUtc(item.FechaFin);
        }

        // Devuelve el id del estado por su nombre (ej. "Pendiente", "Completado").
        private async Task<int> EstadoIdPorNombreAsync(string nombre)
        {
            var estado = await _context.Estados.FirstOrDefaultAsync(e => e.NombreEstado == nombre && e.Activo);
            if (estado == null)
                throw new InvalidOperationException($"No existe el estado '{nombre}' en el catálogo.");
            return estado.Id;
        }

        // Número de informe automático y secuencial POR EMPRESA.
        // La empresa se obtiene del área de la instalación.
        private async Task<string> SiguienteNumeroInformeAsync(int areaId)
        {
            var area = await _context.AreasEmpresa.FindAsync(areaId);
            if (area == null) return string.Empty;
            var seqs = await _context.Database
                .SqlQueryRaw<int>(
                    "UPDATE public.empresa SET numero_informe_seq = numero_informe_seq + 1 WHERE id = {0} RETURNING numero_informe_seq AS \"Value\"",
                    area.EmpresaId)
                .ToListAsync();
            return seqs.FirstOrDefault().ToString("D4");
        }

        // Reemplaza los colaboradores (tecnicos adicionales) de la instalacion.
        // ids == null => no se toca; excluye siempre al responsable.
        private async Task SincronizarColaboradoresAsync(int instalacionId, int responsableId, List<int>? ids)
        {
            if (ids == null) return;
            var deseados = ids.Where(uid => uid != responsableId).Distinct().ToList();
            var actuales = await _context.InstalacionTecnicos.Where(t => t.InstalacionId == instalacionId).ToListAsync();

            var aEliminar = actuales.Where(a => !deseados.Contains(a.UsuarioId)).ToList();
            if (aEliminar.Count > 0) _context.InstalacionTecnicos.RemoveRange(aEliminar);

            var existentes = actuales.Select(a => a.UsuarioId).ToHashSet();
            foreach (var uid in deseados.Where(uid => !existentes.Contains(uid)))
                _context.InstalacionTecnicos.Add(new Instalacion_Tecnico { InstalacionId = instalacionId, UsuarioId = uid });

            await _context.SaveChangesAsync();
        }

        // Responsable + colaboradores: destinatarios de las notificaciones del servicio.
        private async Task<List<int>> TecnicoIdsAsync(int instalacionId, int responsableId)
        {
            var colabs = await _context.InstalacionTecnicos
                .Where(t => t.InstalacionId == instalacionId).Select(t => t.UsuarioId).ToListAsync();
            return new[] { responsableId }.Concat(colabs).Distinct().ToList();
        }

        public async Task<Instalacion> CreateAsync(Instalacion item, string usuarioRegistro)
        {
            NormalizarFechas(item);
            item.Activo = true;
            // Estado automático: toda instalación nace en "Pendiente".
            item.EstadoId = await EstadoIdPorNombreAsync("Pendiente");
            // Número de informe automático por empresa (si no vino uno manual).
            if (string.IsNullOrWhiteSpace(item.NumeroInforme))
                item.NumeroInforme = await SiguienteNumeroInformeAsync(item.AreaId);
            item.UsuarioRegistro = usuarioRegistro;
            item.FechaRegistro = DateTime.UtcNow;
            item.IpRegistro = _auditoriaService.ObtenerIp();

            _context.Instalaciones.Add(item);
            await _context.SaveChangesAsync();

            await SincronizarColaboradoresAsync(item.Id, item.TecnicoId, item.ColaboradorIds);

            // Notificación in-app: quienes pueden aprobar (supervisores y
            // administradores) deben enterarse del nuevo servicio.
            await _notificacionService.NotificarNuevoServicioAsync(
                "instalación", "instalacion", item.Id,
                string.IsNullOrEmpty(item.NumeroInforme) ? $"#{item.Id}" : item.NumeroInforme,
                null, usuarioRegistro);

            return item;
        }

        // Aprueba la instalación: pasa su estado a "Completado".
        public async Task<bool> AprobarAsync(int id, string usuario)
        {
            var item = await _context.Instalaciones.FindAsync(id);
            if (item == null) return false;

            item.EstadoId = await EstadoIdPorNombreAsync("Completado");
            item.UsuarioModificacion = usuario;
            item.FechaModificacion = DateTime.UtcNow;
            item.IpModificacion = _auditoriaService.ObtenerIp();
            await _context.SaveChangesAsync();

            try
            {
                foreach (var uid in await TecnicoIdsAsync(item.Id, item.TecnicoId))
                    await _notificacionService.CreateAsync(new Notificacion
                    {
                        UsuarioId = uid,
                        Titulo = "Instalación Completada",
                        Mensaje = $"La instalación {(string.IsNullOrEmpty(item.NumeroInforme) ? $"#{item.Id}" : item.NumeroInforme)} fue aprobada y marcada como Completada.",
                        Tipo = "completado",
                        Origen = "instalacion",
                        ReferenciaId = item.Id,
                    }, usuario);
            }
            catch (Exception ex) { Console.WriteLine($"Error notificación aprobación instalación: {ex.GetBaseException().Message}"); }

            return true;
        }

        public async Task<bool> UpdateAsync(int id, Instalacion item, string usuarioModificacion)
        {
            var existingItem = await _context.Instalaciones.FindAsync(id);
            if (existingItem == null) return false;

            var estadoAnteriorId = existingItem.EstadoId;

            NormalizarFechas(item);
            var entry = _context.Entry(existingItem);
            entry.CurrentValues.SetValues(item);
            EntityUpdateHelper.PreservarCamposRegistro(entry);
            EntityUpdateHelper.PreservarSiVacio(entry, "NumeroInforme");
            existingItem.UsuarioModificacion = usuarioModificacion;
            existingItem.FechaModificacion = DateTime.UtcNow;
            existingItem.IpModificacion = _auditoriaService.ObtenerIp();

            await _context.SaveChangesAsync();

            await SincronizarColaboradoresAsync(existingItem.Id, existingItem.TecnicoId, item.ColaboradorIds);

            // Notificación in-app al técnico (responsable y colaboradores) cuando cambia el estado
            if (existingItem.EstadoId != estadoAnteriorId)
            {
                try
                {
                    var estado = await _context.Estados.FindAsync(existingItem.EstadoId);
                    var nombreEstado = estado?.NombreEstado ?? "Actualizada";
                    var informe = string.IsNullOrEmpty(existingItem.NumeroInforme) ? $"#{existingItem.Id}" : existingItem.NumeroInforme;

                    foreach (var uid in await TecnicoIdsAsync(existingItem.Id, existingItem.TecnicoId))
                        await _notificacionService.CreateAsync(new Notificacion
                        {
                            UsuarioId = uid,
                            Titulo = $"Instalación {nombreEstado}",
                            Mensaje = $"La instalación {informe} cambió de estado a \"{nombreEstado}\".",
                            Tipo = NotificacionService.TipoPorEstado(nombreEstado),
                            Origen = "instalacion",
                            ReferenciaId = existingItem.Id,
                        }, usuarioModificacion);

                    // Si quedo esperando aprobacion, avisar tambien a los supervisores.
                    if (nombreEstado.Contains("Esperando", StringComparison.OrdinalIgnoreCase))
                    {
                        var tecnico = await _context.Usuarios.FindAsync(existingItem.TecnicoId);
                        await _notificacionService.NotificarPendienteAprobacionAsync(
                            "instalación", "instalacion", existingItem.Id, informe,
                            tecnico == null ? "" : $"{tecnico.Nombre} {tecnico.Apellido}".Trim(),
                            "", usuarioModificacion);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error creando notificación de instalación: {ex.GetBaseException().Message}");
                }
            }

            return true;
        }

        public async Task<bool> DeleteAsync(int id, string usuarioEliminacion)
        {
            var existingItem = await _context.Instalaciones.FindAsync(id);
            if (existingItem == null) return false;

            existingItem.Activo = false;
            existingItem.UsuarioEliminacion = usuarioEliminacion;
            existingItem.FechaEliminacion = DateTime.UtcNow;
            existingItem.IpEliminacion = _auditoriaService.ObtenerIp();

            await _context.SaveChangesAsync();
            return true;
        }

        // Reactiva un registro previamente desactivado (activo = true).
        public async Task<bool> ReactivarAsync(int id, string usuario)
        {
            var item = await _context.Instalaciones.FindAsync(id);
            if (item == null) return false;

            item.Activo = true;
            item.UsuarioModificacion = usuario;
            item.FechaModificacion = DateTime.UtcNow;
            item.IpModificacion = _auditoriaService.ObtenerIp();

            await _context.SaveChangesAsync();
            return true;
        }

    }
}
