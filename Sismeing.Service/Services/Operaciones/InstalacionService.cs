using Microsoft.EntityFrameworkCore;
using Sismeing.Domain.Entities.Operaciones;
using Sismeing.Infrestructura.Persistence;
using Sismeing.Service.Interfaces.Operaciones;

namespace Sismeing.Service.Services.Operaciones
{
    public class InstalacionService : IInstalacionService
    {
        private readonly SupaBaseDBcontext _context;

        public InstalacionService(SupaBaseDBcontext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Instalacion>> GetAllAsync()
        {
            return await _context.Instalaciones.ToListAsync();
        }

        public async Task<Instalacion?> GetByIdAsync(int id)
        {
            return await _context.Instalaciones.FindAsync(id);
        }

        public async Task<Instalacion> CreateAsync(Instalacion item, string usuarioRegistro)
        {
            item.UsuarioRegistro = usuarioRegistro;
            item.FechaRegistro = DateTime.UtcNow;
            item.Activo = true;

            _context.Instalaciones.Add(item);
            await _context.SaveChangesAsync();

            return item;
        }

        public async Task<bool> UpdateAsync(int id, Instalacion item, string usuarioModificacion)
        {
            var existingItem = await _context.Instalaciones.FindAsync(id);
            if (existingItem == null) return false;

            _context.Entry(existingItem).CurrentValues.SetValues(item);
            existingItem.UsuarioModificacion = usuarioModificacion;
            existingItem.FechaModificacion = DateTime.UtcNow;
            
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id, string usuarioEliminacion)
        {
            var existingItem = await _context.Instalaciones.FindAsync(id);
            if (existingItem == null) return false;

            existingItem.Activo = false;
            existingItem.UsuarioEliminacion = usuarioEliminacion;
            existingItem.FechaEliminacion = DateTime.UtcNow;
            
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
