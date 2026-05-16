using Microsoft.EntityFrameworkCore;
using Sismeing.Domain.Entities.Catalogo;
using Sismeing.Infrestructura.Persistence;
using Sismeing.Service.Interfaces.Catalogo;

namespace Sismeing.Service.Services.Catalogo
{
    public class EstadoService : IEstadoService
    {
        private readonly SupaBaseDBcontext _context;

        public EstadoService(SupaBaseDBcontext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Estado>> GetAllAsync()
        {
            return await _context.Estados.ToListAsync();
        }

        public async Task<Estado?> GetByIdAsync(int id)
        {
            return await _context.Estados.FindAsync(id);
        }

        public async Task<Estado> CreateAsync(Estado item, string usuarioRegistro)
        {
            item.UsuarioRegistro = usuarioRegistro;
            item.FechaRegistro = DateTime.UtcNow;
            item.Activo = true;

            _context.Estados.Add(item);
            await _context.SaveChangesAsync();

            return item;
        }

        public async Task<bool> UpdateAsync(int id, Estado item, string usuarioModificacion)
        {
            var existingItem = await _context.Estados.FindAsync(id);
            if (existingItem == null) return false;

            _context.Entry(existingItem).CurrentValues.SetValues(item);
            existingItem.UsuarioModificacion = usuarioModificacion;
            existingItem.FechaModificacion = DateTime.UtcNow;
            
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id, string usuarioEliminacion)
        {
            var existingItem = await _context.Estados.FindAsync(id);
            if (existingItem == null) return false;

            existingItem.Activo = false;
            existingItem.UsuarioEliminacion = usuarioEliminacion;
            existingItem.FechaEliminacion = DateTime.UtcNow;
            
            await _context.SaveChangesAsync();
            return true;
        }
    }
}