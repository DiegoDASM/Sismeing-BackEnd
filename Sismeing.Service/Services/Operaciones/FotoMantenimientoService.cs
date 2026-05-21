using Microsoft.EntityFrameworkCore;
using Sismeing.Domain.Entities.Operaciones;
using Sismeing.Infrestructura.Persistence;
using Sismeing.Service.Interfaces.Operaciones;

namespace Sismeing.Service.Services.Operaciones
{
    public class FotoMantenimientoService : IFoto_MantenimientoService
    {
        private readonly SupaBaseDBcontext _context;

        public FotoMantenimientoService(SupaBaseDBcontext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Foto_Mantenimiento>> GetAllAsync()
        {
            return await _context.FotosMantenimiento.ToListAsync();
        }

        public async Task<Foto_Mantenimiento?> GetByIdAsync(int id)
        {
            return await _context.FotosMantenimiento.FindAsync(id);
        }

        public async Task<Foto_Mantenimiento> CreateAsync(Foto_Mantenimiento item, string usuarioRegistro)
        {
            item.UsuarioRegistro = usuarioRegistro;
            item.FechaRegistro = DateTime.UtcNow;
            item.Activo = true;

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
            
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
