using Microsoft.EntityFrameworkCore;
using Sismeing.Domain.Entities.Operaciones;
using Sismeing.Infrestructura.Persistence;
using Sismeing.Service.Interfaces.Operaciones;

namespace Sismeing.Service.Services.Operaciones
{
    public class MedicionService : IMedicionService
    {
        private readonly SupaBaseDBcontext _context;

        public MedicionService(SupaBaseDBcontext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Medicion>> GetAllAsync()
        {
            return await _context.Mediciones.ToListAsync();
        }

        public async Task<Medicion?> GetByIdAsync(int id)
        {
            return await _context.Mediciones.FindAsync(id);
        }

        public async Task<Medicion> CreateAsync(Medicion item, string usuarioRegistro)
        {
            item.UsuarioRegistro = usuarioRegistro;
            item.FechaRegistro = DateTime.UtcNow;
            item.Activo = true;

            _context.Mediciones.Add(item);
            await _context.SaveChangesAsync();

            return item;
        }

        public async Task<bool> UpdateAsync(int id, Medicion item, string usuarioModificacion)
        {
            var existingItem = await _context.Mediciones.FindAsync(id);
            if (existingItem == null) return false;

            _context.Entry(existingItem).CurrentValues.SetValues(item);
            existingItem.UsuarioModificacion = usuarioModificacion;
            existingItem.FechaModificacion = DateTime.UtcNow;
            
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id, string usuarioEliminacion)
        {
            var existingItem = await _context.Mediciones.FindAsync(id);
            if (existingItem == null) return false;

            existingItem.Activo = false;
            existingItem.UsuarioEliminacion = usuarioEliminacion;
            existingItem.FechaEliminacion = DateTime.UtcNow;
            
            await _context.SaveChangesAsync();
            return true;
        }
    }
}