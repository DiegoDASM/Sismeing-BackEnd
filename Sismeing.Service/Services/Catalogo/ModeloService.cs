using Microsoft.EntityFrameworkCore;
using Sismeing.Domain.Entities.Catalogo;
using Sismeing.Infrestructura.Persistence;
using Sismeing.Service.Interfaces.Catalogo;

namespace Sismeing.Service.Services.Catalogo
{
    public class ModeloService : IModeloService
    {
        private readonly SupaBaseDBcontext _context;

        public ModeloService(SupaBaseDBcontext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Modelo>> GetAllAsync()
        {
            return await _context.Modelos.ToListAsync();
        }

        public async Task<Modelo?> GetByIdAsync(int id)
        {
            return await _context.Modelos.FindAsync(id);
        }

        public async Task<Modelo> CreateAsync(Modelo item, string usuarioRegistro)
        {
            item.UsuarioRegistro = usuarioRegistro;
            item.FechaRegistro = DateTime.UtcNow;
            item.Activo = true;

            _context.Modelos.Add(item);
            await _context.SaveChangesAsync();

            return item;
        }

        public async Task<bool> UpdateAsync(int id, Modelo item, string usuarioModificacion)
        {
            var existingItem = await _context.Modelos.FindAsync(id);
            if (existingItem == null) return false;

            _context.Entry(existingItem).CurrentValues.SetValues(item);
            existingItem.UsuarioModificacion = usuarioModificacion;
            existingItem.FechaModificacion = DateTime.UtcNow;
            
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id, string usuarioEliminacion)
        {
            var existingItem = await _context.Modelos.FindAsync(id);
            if (existingItem == null) return false;

            existingItem.Activo = false;
            existingItem.UsuarioEliminacion = usuarioEliminacion;
            existingItem.FechaEliminacion = DateTime.UtcNow;
            
            await _context.SaveChangesAsync();
            return true;
        }
    }
}