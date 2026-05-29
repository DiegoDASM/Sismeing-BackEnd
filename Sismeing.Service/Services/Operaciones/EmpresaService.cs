using Microsoft.EntityFrameworkCore;
using Sismeing.Domain.Entities.Operaciones;
using Sismeing.Infrestructura.Persistence;
using Sismeing.Service.Interfaces.Operaciones;

namespace Sismeing.Service.Services.Operaciones
{
    public class EmpresaService : IEmpresaService
    {
        private readonly SupaBaseDBcontext _context;

        public EmpresaService(SupaBaseDBcontext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Empresa>> GetAllAsync()
        {
            return await _context.Empresas.ToListAsync();
        }

        public async Task<Empresa?> GetByIdAsync(int id)
        {
            return await _context.Empresas.FindAsync(id);
        }

        public async Task<Empresa> CreateAsync(Empresa item, string usuarioRegistro)
        {
            item.UsuarioRegistro = usuarioRegistro;
            item.FechaRegistro = DateTime.UtcNow;
            item.Activo = true;

            _context.Empresas.Add(item);
            await _context.SaveChangesAsync();

            return item;
        }

        public async Task<bool> UpdateAsync(int id, Empresa item, string usuarioModificacion)
        {
            var existingItem = await _context.Empresas.FindAsync(id);
            if (existingItem == null) return false;

            _context.Entry(existingItem).CurrentValues.SetValues(item);
            existingItem.UsuarioModificacion = usuarioModificacion;
            existingItem.FechaModificacion = DateTime.UtcNow;
            
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id, string usuarioEliminacion)
        {
            var existingItem = await _context.Empresas.FindAsync(id);
            if (existingItem == null) return false;

            existingItem.Activo = false;
            existingItem.UsuarioEliminacion = usuarioEliminacion;
            existingItem.FechaEliminacion = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<string?> UpdateLogoAsync(int id, string logoPath)
        {
            var empresa = await _context.Empresas.FindAsync(id);
            if (empresa == null) return null;

            empresa.Logo = logoPath;
            await _context.SaveChangesAsync();
            return logoPath;
        }
    }
}