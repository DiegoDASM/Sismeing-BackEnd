using Microsoft.EntityFrameworkCore;
using Sismeing.Domain.Entities.Catalogo;
using Sismeing.Infrestructura.Persistence;
using Sismeing.Service.Interfaces.Catalogo;

namespace Sismeing.Service.Services.Catalogo
{
    public class Tipo_EquipoService : ITipo_EquipoService
    {
        private readonly SupaBaseDBcontext _context;

        public Tipo_EquipoService(SupaBaseDBcontext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Tipo_Equipo>> GetAllAsync()
        {
            return await _context.TiposEquipo.ToListAsync();
        }

        public async Task<Tipo_Equipo?> GetByIdAsync(int id)
        {
            return await _context.TiposEquipo.FindAsync(id);
        }

        public async Task<Tipo_Equipo> CreateAsync(Tipo_Equipo item, string usuarioRegistro)
        {
            item.UsuarioRegistro = usuarioRegistro;
            item.FechaRegistro = DateTime.UtcNow;
            item.Activo = true;

            _context.TiposEquipo.Add(item);
            await _context.SaveChangesAsync();

            return item;
        }

        public async Task<bool> UpdateAsync(int id, Tipo_Equipo item, string usuarioModificacion)
        {
            var existingItem = await _context.TiposEquipo.FindAsync(id);
            if (existingItem == null) return false;

            _context.Entry(existingItem).CurrentValues.SetValues(item);
            existingItem.UsuarioModificacion = usuarioModificacion;
            existingItem.FechaModificacion = DateTime.UtcNow;
            
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id, string usuarioEliminacion)
        {
            var existingItem = await _context.TiposEquipo.FindAsync(id);
            if (existingItem == null) return false;

            existingItem.Activo = false;
            existingItem.UsuarioEliminacion = usuarioEliminacion;
            existingItem.FechaEliminacion = DateTime.UtcNow;
            
            await _context.SaveChangesAsync();
            return true;
        }
    }
}