using Microsoft.EntityFrameworkCore;
using Sismeing.Domain.Entities.Operaciones;
using Sismeing.Infrestructura.Persistence;
using Sismeing.Service.Interfaces.Comunes;
using Sismeing.Service.Interfaces.Operaciones;

namespace Sismeing.Service.Services.Operaciones
{
    public class DireccionEmpresaService : IDireccion_EmpresaService
    {
        private readonly SupaBaseDBcontext _context;
        private readonly IAuditoriaService _auditoriaService;

        public DireccionEmpresaService(SupaBaseDBcontext context, IAuditoriaService auditoriaService)
        {
            _context = context;
            _auditoriaService = auditoriaService;
        }

        public async Task<IEnumerable<Direccion_Empresa>> GetAllAsync()
        {
            return await _context.DireccionesEmpresa.ToListAsync();
        }

        public async Task<Direccion_Empresa?> GetByIdAsync(int id)
        {
            return await _context.DireccionesEmpresa.FindAsync(id);
        }

        public async Task<IEnumerable<Direccion_Empresa>> GetByEmpresaIdAsync(int empresaId)
        {
            return await _context.DireccionesEmpresa
                .Where(d => d.EmpresaId == empresaId && d.Activo)
                .ToListAsync();
        }

        public async Task<Direccion_Empresa> CreateAsync(Direccion_Empresa item, string usuarioRegistro)
        {
            item.Activo = true;
            item.UsuarioRegistro = usuarioRegistro;
            item.FechaRegistro = DateTime.UtcNow;
            item.IpRegistro = _auditoriaService.ObtenerIp();

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
            existingItem.IpModificacion = _auditoriaService.ObtenerIp();

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
            existingItem.IpEliminacion = _auditoriaService.ObtenerIp();

            await _context.SaveChangesAsync();
            return true;
        }
    }
}
