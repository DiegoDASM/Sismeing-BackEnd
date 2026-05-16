using Microsoft.EntityFrameworkCore;
using Sismeing.Domain.Entities.Operaciones;
using Sismeing.Infrestructura.Persistence;
using Sismeing.Service.Interfaces.Operaciones;

namespace Sismeing.Service.Services.Operaciones
{
    public class ContratoService : IContratoService
    {
        private readonly SupaBaseDBcontext _context;

        public ContratoService(SupaBaseDBcontext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Contrato>> GetAllAsync()
        {
            return await _context.Contratos.ToListAsync();
        }

        public async Task<Contrato?> GetByIdAsync(int id)
        {
            return await _context.Contratos.FindAsync(id);
        }

        public async Task<Contrato> CreateAsync(Contrato item, string usuarioRegistro)
        {
            item.UsuarioRegistro = usuarioRegistro;
            item.FechaRegistro = DateTime.UtcNow;
            item.Activo = true;

            _context.Contratos.Add(item);
            await _context.SaveChangesAsync();

            return item;
        }

        public async Task<bool> UpdateAsync(int id, Contrato item, string usuarioModificacion)
        {
            var existingItem = await _context.Contratos.FindAsync(id);
            if (existingItem == null) return false;

            _context.Entry(existingItem).CurrentValues.SetValues(item);
            existingItem.UsuarioModificacion = usuarioModificacion;
            existingItem.FechaModificacion = DateTime.UtcNow;
            
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id, string usuarioEliminacion)
        {
            var existingItem = await _context.Contratos.FindAsync(id);
            if (existingItem == null) return false;

            existingItem.Activo = false;
            existingItem.UsuarioEliminacion = usuarioEliminacion;
            existingItem.FechaEliminacion = DateTime.UtcNow;
            
            await _context.SaveChangesAsync();
            return true;
        }
    }
}