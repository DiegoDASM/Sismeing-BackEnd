using Microsoft.EntityFrameworkCore;
using Sismeing.Domain.Entities.Catalogo;
using Sismeing.Infrestructura.Persistence;
using Sismeing.Service.Interfaces.Catalogo;

namespace Sismeing.Service.Services.Catalogo
{
    public class Tipo_TrabajoService : ITipo_TrabajoService
    {
        private readonly SupaBaseDBcontext _context;

        public Tipo_TrabajoService(SupaBaseDBcontext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Tipo_Trabajo>> GetAllAsync()
        {
            return await _context.TiposTrabajo.ToListAsync();
        }

        public async Task<Tipo_Trabajo?> GetByIdAsync(int id)
        {
            return await _context.TiposTrabajo.FindAsync(id);
        }

        public async Task<Tipo_Trabajo> CreateAsync(Tipo_Trabajo item, string usuarioRegistro)
        {
            item.UsuarioRegistro = usuarioRegistro;
            item.FechaRegistro = DateTime.UtcNow;
            item.Activo = true;

            _context.TiposTrabajo.Add(item);
            await _context.SaveChangesAsync();

            return item;
        }

        public async Task<bool> UpdateAsync(int id, Tipo_Trabajo item, string usuarioModificacion)
        {
            var existingItem = await _context.TiposTrabajo.FindAsync(id);
            if (existingItem == null) return false;

            _context.Entry(existingItem).CurrentValues.SetValues(item);
            existingItem.UsuarioModificacion = usuarioModificacion;
            existingItem.FechaModificacion = DateTime.UtcNow;
            
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id, string usuarioEliminacion)
        {
            var existingItem = await _context.TiposTrabajo.FindAsync(id);
            if (existingItem == null) return false;

            existingItem.Activo = false;
            existingItem.UsuarioEliminacion = usuarioEliminacion;
            existingItem.FechaEliminacion = DateTime.UtcNow;
            
            await _context.SaveChangesAsync();
            return true;
        }
    }
}