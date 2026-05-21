using Microsoft.EntityFrameworkCore;
using Sismeing.Domain.Entities.Catalogo;
using Sismeing.Infrestructura.Persistence;
using Sismeing.Service.Interfaces.Catalogo;

namespace Sismeing.Service.Services.Catalogo
{
    public class Tipo_InformeService : ITipo_InformeService
    {
        private readonly SupaBaseDBcontext _context;

        public Tipo_InformeService(SupaBaseDBcontext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Tipo_Informe>> GetAllAsync()
        {
            return await _context.TiposInforme.ToListAsync();
        }

        public async Task<Tipo_Informe?> GetByIdAsync(int id)
        {
            return await _context.TiposInforme.FindAsync(id);
        }

        public async Task<Tipo_Informe> CreateAsync(Tipo_Informe item, string usuarioRegistro)
        {
            item.UsuarioRegistro = usuarioRegistro;
            item.FechaRegistro = DateTime.UtcNow;
            item.Activo = true;

            _context.TiposInforme.Add(item);
            await _context.SaveChangesAsync();

            return item;
        }

        public async Task<bool> UpdateAsync(int id, Tipo_Informe item, string usuarioModificacion)
        {
            var existingItem = await _context.TiposInforme.FindAsync(id);
            if (existingItem == null) return false;

            _context.Entry(existingItem).CurrentValues.SetValues(item);
            existingItem.UsuarioModificacion = usuarioModificacion;
            existingItem.FechaModificacion = DateTime.UtcNow;
            
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id, string usuarioEliminacion)
        {
            var existingItem = await _context.TiposInforme.FindAsync(id);
            if (existingItem == null) return false;

            existingItem.Activo = false;
            existingItem.UsuarioEliminacion = usuarioEliminacion;
            existingItem.FechaEliminacion = DateTime.UtcNow;
            
            await _context.SaveChangesAsync();
            return true;
        }
    }
}