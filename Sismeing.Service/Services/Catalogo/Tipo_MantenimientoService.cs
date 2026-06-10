using Microsoft.EntityFrameworkCore;
using Sismeing.Domain.Entities.Catalogo;
using Sismeing.Infrestructura.Persistence;
using Sismeing.Service.Interfaces.Catalogo;
using Sismeing.Service.Interfaces.Comunes;

namespace Sismeing.Service.Services.Catalogo
{
    public class Tipo_MantenimientoService : ITipo_MantenimientoService
    {
        private readonly SupaBaseDBcontext _context;
        private readonly IAuditoriaService _auditoriaService;

        public Tipo_MantenimientoService(SupaBaseDBcontext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Tipo_Mantenimiento>> GetAllAsync()
        {
            return await _context.TiposMantenimiento.ToListAsync();
        }

        public async Task<Tipo_Mantenimiento?> GetByIdAsync(int id)
        {
            return await _context.TiposMantenimiento.FindAsync(id);
        }

        public async Task<Tipo_Mantenimiento> CreateAsync(Tipo_Mantenimiento item, string usuarioRegistro)
        {
            item.Activo = true;
            item.UsuarioRegistro = usuarioRegistro;
            item.FechaRegistro = DateTime.UtcNow;
            item.IpRegistro = _auditoriaService.ObtenerIp();

            _context.TiposMantenimiento.Add(item);
            await _context.SaveChangesAsync();

            return item;
        }

        public async Task<bool> UpdateAsync(int id, Tipo_Mantenimiento item, string usuarioModificacion)
        {
            var existingItem = await _context.TiposMantenimiento.FindAsync(id);
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
            var existingItem = await _context.TiposMantenimiento.FindAsync(id);
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