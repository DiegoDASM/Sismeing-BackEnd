using Microsoft.EntityFrameworkCore;
using Sismeing.Domain.Entities.Operaciones;
using Sismeing.Infrestructura.Persistence;
using Sismeing.Service.Interfaces.Comunes;
using Sismeing.Service.Interfaces.Operaciones;

namespace Sismeing.Service.Services.Operaciones
{
    public class NotificacionService : INotificacionService
    {
        private readonly SupaBaseDBcontext _context;
        private readonly IAuditoriaService _auditoriaService;

        public NotificacionService(SupaBaseDBcontext context, IAuditoriaService auditoriaService)
        {
            _context = context;
            _auditoriaService = auditoriaService;
        }

        // Mapea el nombre de un estado del catálogo al tipo de badge del frontend.
        // Catálogo actual: Pendiente, En progreso, Completado, Cancelado,
        // Requiere repuestos, Esperando aprobación.
        // "Esperando aprobación" se evalúa antes que "aprobado" a propósito.
        public static string TipoPorEstado(string nombreEstado)
        {
            var lower = nombreEstado.ToLowerInvariant();
            if (lower.Contains("esper") || lower.Contains("pend") || lower.Contains("revis") || lower.Contains("repuesto")) return "pendiente";
            if (lower.Contains("aprob")) return "aprobado";
            if (lower.Contains("complet") || lower.Contains("final")) return "completado";
            if (lower.Contains("program") || lower.Contains("proceso") || lower.Contains("progres")) return "programado";
            return "info";
        }

        public async Task<IEnumerable<Notificacion>> GetByUsuarioAsync(int usuarioId)
        {
            return await _context.Notificaciones
                .Where(n => n.UsuarioId == usuarioId && n.Activo)
                .OrderByDescending(n => n.FechaRegistro)
                .ToListAsync();
        }

        public async Task<int> GetNoLeidasCountAsync(int usuarioId)
        {
            return await _context.Notificaciones
                .CountAsync(n => n.UsuarioId == usuarioId && n.Activo && !n.Leida);
        }

        public async Task<Notificacion> CreateAsync(Notificacion item, string usuarioRegistro)
        {
            item.Activo = true;
            item.Leida = false;
            item.UsuarioRegistro = usuarioRegistro;
            item.FechaRegistro = DateTime.UtcNow;
            item.IpRegistro = _auditoriaService.ObtenerIp();

            _context.Notificaciones.Add(item);
            await _context.SaveChangesAsync();

            return item;
        }

        public async Task<bool> MarcarLeidaAsync(int id, string usuarioModificacion)
        {
            var existingItem = await _context.Notificaciones.FindAsync(id);
            if (existingItem == null) return false;

            existingItem.Leida = true;
            existingItem.FechaLeida = DateTime.UtcNow;
            existingItem.UsuarioModificacion = usuarioModificacion;
            existingItem.FechaModificacion = DateTime.UtcNow;
            existingItem.IpModificacion = _auditoriaService.ObtenerIp();

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<int> MarcarTodasLeidasAsync(int usuarioId, string usuarioModificacion)
        {
            var pendientes = await _context.Notificaciones
                .Where(n => n.UsuarioId == usuarioId && n.Activo && !n.Leida)
                .ToListAsync();

            var ahora = DateTime.UtcNow;
            var ip = _auditoriaService.ObtenerIp();
            foreach (var n in pendientes)
            {
                n.Leida = true;
                n.FechaLeida = ahora;
                n.UsuarioModificacion = usuarioModificacion;
                n.FechaModificacion = ahora;
                n.IpModificacion = ip;
            }

            await _context.SaveChangesAsync();
            return pendientes.Count;
        }

        public async Task<bool> DeleteAsync(int id, string usuarioEliminacion)
        {
            var existingItem = await _context.Notificaciones.FindAsync(id);
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
