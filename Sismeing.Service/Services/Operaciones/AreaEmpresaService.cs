using Microsoft.EntityFrameworkCore;
using Sismeing.Domain.Entities.Operaciones;
using Sismeing.Infrestructura.Persistence;
using Sismeing.Service.Interfaces.Comunes;
using Sismeing.Service.Interfaces.Operaciones;

namespace Sismeing.Service.Services.Operaciones
{
    public class AreaEmpresaService : IArea_EmpresaService
    {
        private readonly SupaBaseDBcontext _context;
        private readonly IAuditoriaService _auditoriaService;

        public AreaEmpresaService(SupaBaseDBcontext context, IAuditoriaService auditoriaService)
        {
            _context = context;
            _auditoriaService = auditoriaService;
        }

        public async Task<IEnumerable<Area_Empresa>> GetAllAsync()
        {
            return await _context.AreasEmpresa.ToListAsync();
        }

        public async Task<Area_Empresa?> GetByIdAsync(int id)
        {
            return await _context.AreasEmpresa.FindAsync(id);
        }

        public async Task<IEnumerable<Area_Empresa>> GetByEmpresaIdAsync(int empresaId)
        {
            return await _context.AreasEmpresa
                .Include(a => a.DireccionEmpresa)
                .Where(a => a.EmpresaId == empresaId && a.Activo)
                .ToListAsync();
        }

        public async Task<IEnumerable<Area_Empresa>> GetByDireccionIdAsync(int direccionId)
        {
            return await _context.AreasEmpresa
                .Where(a => a.DireccionEmpresaId == direccionId && a.Activo)
                .ToListAsync();
        }

        public async Task<Area_Empresa> CreateAsync(Area_Empresa item, string usuarioRegistro)
        {
            item.Activo = true;
            item.UsuarioRegistro = usuarioRegistro;
            item.FechaRegistro = DateTime.UtcNow;
            item.IpRegistro = _auditoriaService.ObtenerIp();

            _context.AreasEmpresa.Add(item);
            await _context.SaveChangesAsync();

            return item;
        }

        public async Task<bool> UpdateAsync(int id, Area_Empresa item, string usuarioModificacion)
        {
            var existingItem = await _context.AreasEmpresa.FindAsync(id);
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
            var existingItem = await _context.AreasEmpresa.FindAsync(id);
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
