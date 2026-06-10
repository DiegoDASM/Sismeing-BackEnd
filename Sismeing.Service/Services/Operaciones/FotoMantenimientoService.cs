using Microsoft.EntityFrameworkCore;
using Sismeing.Domain.Entities.Operaciones;
using Sismeing.Infrestructura.Persistence;
using Sismeing.Service.Interfaces.Comunes;
using Sismeing.Service.Interfaces.Operaciones;

namespace Sismeing.Service.Services.Operaciones
{
    public class FotoMantenimientoService : IFoto_MantenimientoService
    {
        private readonly SupaBaseDBcontext _context;
        private readonly IAuditoriaService _auditoriaService;

        public FotoMantenimientoService(SupaBaseDBcontext context, IAuditoriaService auditoriaService)
        {
            _context = context;
            _auditoriaService = auditoriaService;
        }

        public async Task<IEnumerable<Foto_Mantenimiento>> GetAllAsync()
        {
            return await _context.FotosMantenimiento.ToListAsync();
        }

        public async Task<Foto_Mantenimiento?> GetByIdAsync(int id)
        {
            return await _context.FotosMantenimiento.FindAsync(id);
        }

        public async Task<IEnumerable<Foto_Mantenimiento>> GetByMantenimientoIdAsync(int mantenimientoId)
        {
            return await _context.FotosMantenimiento
                .Where(f => f.MantenimientoId == mantenimientoId && f.Activo)
                .OrderBy(f => f.Tipo)
                .ThenBy(f => f.FechaRegistro)
                .ToListAsync();
        }

        public async Task<Foto_Mantenimiento> CreateAsync(Foto_Mantenimiento item, string usuarioRegistro)
        {
            item.UsuarioRegistro = usuarioRegistro;
            item.FechaRegistro = DateTime.UtcNow;
            item.Activo = true;
            item.IpRegistro = _auditoriaService.ObtenerIp();

            _context.FotosMantenimiento.Add(item);
            await _context.SaveChangesAsync();

            return item;
        }

        public async Task<bool> UpdateAsync(int id, Foto_Mantenimiento item, string usuarioModificacion)
        {
            var existingItem = await _context.FotosMantenimiento.FindAsync(id);
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
            var existingItem = await _context.FotosMantenimiento.FindAsync(id);
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
