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

        public InstalacionService(SupaBaseDBcontext context, IAuditoriaService auditoriaService, INotificacionService notificacionService)
        {
            _context = context;
            _auditoriaService = auditoriaService;
            _notificacionService = notificacionService;
        }

        public async Task<IEnumerable<Instalacion>> GetAllAsync()
        {
            return await _context.Instalaciones
                .Include(i => i.Equipo).ThenInclude(e => e.Marca)
                .Include(i => i.Equipo).ThenInclude(e => e.Proyecto)
                .Include(i => i.Area).ThenInclude(a => a.Empresa)
                .Include(i => i.Tecnico)
                .Include(i => i.Estado)
                .ToListAsync();
        }

        public async Task<Instalacion?> GetByIdAsync(int id)
        {
            return await _context.Instalaciones
                .Include(i => i.Equipo).ThenInclude(e => e.Marca)
                .Include(i => i.Equipo).ThenInclude(e => e.Proyecto)
                .Include(i => i.Area).ThenInclude(a => a.Empresa)
                .Include(i => i.Tecnico)
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
                await _notificacionService.CreateAsync(new Notificacion
                {
                    UsuarioId = item.TecnicoId,
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

            // Notificación in-app al técnico cuando cambia el estado
            if (existingItem.EstadoId != estadoAnteriorId)
            {
                try
                {
                    var estado = await _context.Estados.FindAsync(existingItem.EstadoId);
                    var nombreEstado = estado?.NombreEstado ?? "Actualizada";
                    var informe = string.IsNullOrEmpty(existingItem.NumeroInforme) ? $"#{existingItem.Id}" : existingItem.NumeroInforme;

                    await _notificacionService.CreateAsync(new Notificacion
                    {
                        UsuarioId = existingItem.TecnicoId,
                        Titulo = $"Instalación {nombreEstado}",
                        Mensaje = $"La instalación {informe} cambió de estado a \"{nombreEstado}\".",
                        Tipo = NotificacionService.TipoPorEstado(nombreEstado),
                        Origen = "instalacion",
                        ReferenciaId = existingItem.Id,
                    }, usuarioModificacion);
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
