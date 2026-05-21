using Microsoft.EntityFrameworkCore;
using Sismeing.Domain.Entities.Catalogo;
using Sismeing.Infrestructura.Persistence;
using Sismeing.Service.Interfaces.Catalogo;

namespace Sismeing.Service.Services.Catalogo
{
    public class TrabajoService : ITrabajoService
    {
        private readonly SupaBaseDBcontext _context;

        public TrabajoService(SupaBaseDBcontext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Trabajo>> GetAllAsync()
        {
            return await _context.Trabajos.ToListAsync();
        }

        public async Task<Trabajo?> GetByIdAsync(int id)
        {
            return await _context.Trabajos.FindAsync(id);
        }

        public async Task<Trabajo> CreateAsync(Trabajo item, string usuarioRegistro)
        {
            item.UsuarioRegistro = usuarioRegistro;
            item.FechaRegistro = DateTime.UtcNow;
            item.Activo = true;

            _context.Trabajos.Add(item);
            await _context.SaveChangesAsync();

            return item;
        }

        public async Task<bool> UpdateAsync(int id, Trabajo item, string usuarioModificacion)
        {
            var existingItem = await _context.Trabajos.FindAsync(id);
            if (existingItem == null) return false;

            _context.Entry(existingItem).CurrentValues.SetValues(item);
            existingItem.UsuarioModificacion = usuarioModificacion;
            existingItem.FechaModificacion = DateTime.UtcNow;
            
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
            
            await _context.SaveChangesAsync();
            return true;
        }
    }
}