using Microsoft.EntityFrameworkCore;
using Sismeing.Domain.Entities.Catalogo;
using Sismeing.Infrestructura.Persistence;
using Sismeing.Service.Interfaces.Catalogo;
using Sismeing.Service.Interfaces.Comunes;

namespace Sismeing.Service.Services.Catalogo
{
    public class TrabajoService : ITrabajoService
    {
        private readonly SupaBaseDBcontext _context;
        private readonly IAuditoriaService _auditoriaService;

        public TrabajoService(SupaBaseDBcontext context, IAuditoriaService auditoriaService)
        {
            _context = context;
            _auditoriaService = auditoriaService;
        }

        public async Task<IEnumerable<Trabajo>> GetAllAsync()
        {
            return await _context.Trabajos.ToListAsync();
        }

        public async Task<Trabajo?> GetByIdAsync(int id)
        {
            return await _context.Trabajos.FindAsync(id);
        }

        public async Task<IEnumerable<Trabajo>> GetByMantenimientoAsync(int mantenimientoId)
        {
            return await _context.Trabajos
                .Where(t => t.MantenimientoId == mantenimientoId && t.Activo)
                .OrderBy(t => t.Id)
                .ToListAsync();
        }

        public async Task<Trabajo> CreateAsync(Trabajo item, string usuarioRegistro)
        {
            // No permitir trabajos de catálogo repetidos (sin distinguir mayúsculas).
            // Solo se valida contra los "templates" del catálogo (sin mantenimiento asociado).
            if (item.MantenimientoId == null)
            {
                var nombre = (item.NombreTrabajo ?? "").Trim().ToLower();
                var existe = await _context.Trabajos
                    .AnyAsync(t => t.Activo && t.MantenimientoId == null && t.NombreTrabajo.ToLower() == nombre);
                if (existe)
                    throw new InvalidOperationException($"Ya existe el trabajo '{item.NombreTrabajo}'.");
            }

            item.Activo = true;
            item.UsuarioRegistro = usuarioRegistro;
            item.FechaRegistro = DateTime.UtcNow;
            item.IpRegistro = _auditoriaService.ObtenerIp();

            _context.Trabajos.Add(item);
            await _context.SaveChangesAsync();

            return item;
        }

        /// <summary>
        /// Reemplaza por completo los trabajos realizados de un mantenimiento:
        /// da de baja (lógica) los anteriores y guarda los nuevos. Sirve para crear y editar.
        /// </summary>
        public async Task<IEnumerable<Trabajo>> ReemplazarPorMantenimientoAsync(int mantenimientoId, IEnumerable<Trabajo> items, string usuarioRegistro)
        {
            var now = DateTime.UtcNow;
            var ip = _auditoriaService.ObtenerIp();

            var anteriores = await _context.Trabajos
                .Where(t => t.MantenimientoId == mantenimientoId && t.Activo)
                .ToListAsync();
            foreach (var ant in anteriores)
            {
                ant.Activo = false;
                ant.UsuarioEliminacion = usuarioRegistro;
                ant.FechaEliminacion = now;
                ant.IpEliminacion = ip;
            }

            var nuevos = items
                .Where(t => !string.IsNullOrWhiteSpace(t.NombreTrabajo))
                .Select(t => new Trabajo
                {
                    NombreTrabajo = t.NombreTrabajo,
                    Descripcion = t.Descripcion,
                    MantenimientoId = mantenimientoId,
                    Activo = true,
                    UsuarioRegistro = usuarioRegistro,
                    FechaRegistro = now,
                    IpRegistro = ip,
                })
                .ToList();

            _context.Trabajos.AddRange(nuevos);
            await _context.SaveChangesAsync();
            return nuevos;
        }

        public async Task<bool> UpdateAsync(int id, Trabajo item, string usuarioModificacion)
        {
            var existingItem = await _context.Trabajos.FindAsync(id);
            if (existingItem == null) return false;

            _context.Entry(existingItem).CurrentValues.SetValues(item);
            existingItem.UsuarioModificacion = usuarioModificacion;
            existingItem.FechaModificacion = DateTime.UtcNow;
            existingItem.IpModificacion = _auditoriaService.ObtenerIp();

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id, string usuarioEliminacion)
        {
            var existingItem = await _context.Trabajos.FindAsync(id);
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