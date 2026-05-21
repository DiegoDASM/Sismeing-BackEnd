using Microsoft.EntityFrameworkCore;
using Sismeing.Domain.Entities.Operaciones;
using Sismeing.Infrestructura.Persistence;
using Sismeing.Service.Interfaces.Operaciones;

namespace Sismeing.Service.Services.Operaciones
{
    public class DireccionEmpresaService : IDireccion_EmpresaService
    {
        private readonly SupaBaseDBcontext _context;

        public DireccionEmpresaService(SupaBaseDBcontext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Direccion_Empresa>> GetAllAsync()
        {
            return await _context.DireccionesEmpresa.ToListAsync();
        }

        public async Task<Direccion_Empresa?> GetByIdAsync(int id)
        {
            return await _context.DireccionesEmpresa.FindAsync(id);
        }

        public async Task<Direccion_Empresa> CreateAsync(Direccion_Empresa item, string usuarioRegistro)
        {
            item.UsuarioRegistro = usuarioRegistro;
            item.FechaRegistro = DateTime.UtcNow;
            item.Activo = true;

            _context.DireccionesEmpresa.Add(item);
            await _context.SaveChangesAsync();

            return item;
        }

        public async Task<bool> UpdateAsync(int id, Direccion_Empresa item, string usuarioModificacion)
        {
            var existingItem = await _context.DireccionesEmpresa.FindAsync(id);
            if (existingItem == null) return false;

            _context.Entry(existingItem).CurrentValues.SetValues(item);
            existingItem.UsuarioModificacion = usuarioModificacion;
            existingItem.FechaModificacion = DateTime.UtcNow;
            
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id, string usuarioEliminacion)
        {
            var existingItem = await _context.DireccionesEmpresa.FindAsync(id);
            if (existingItem == null) return false;

            existingItem.Activo = false;
            existingItem.UsuarioEliminacion = usuarioEliminacion;
            existingItem.FechaEliminacion = DateTime.UtcNow;
            
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
