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
                .Include(i => i.Area).ThenInclude(a => a.Empresa)
                .Include(i => i.Tecnico)
                .Include(i => i.Estado)
                .ToListAsync();
        }

        public async Task<Instalacion?> GetByIdAsync(int id)
        {
            return await _context.Instalaciones
                .Include(i => i.Equipo).ThenInclude(e => e.Marca)
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

        public async Task<Instalacion> CreateAsync(Instalacion item, string usuarioRegistro)
        {
            NormalizarFechas(item);
            item.Activo = true;
            item.UsuarioRegistro = usuarioRegistro;
            item.FechaRegistro = DateTime.UtcNow;
            item.IpRegistro = _auditoriaService.ObtenerIp();

            _context.Instalaciones.Add(item);
            await _context.SaveChangesAsync();

            return item;
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
    }
}
