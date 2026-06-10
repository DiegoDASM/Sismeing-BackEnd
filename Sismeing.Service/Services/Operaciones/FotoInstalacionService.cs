using Microsoft.EntityFrameworkCore;
using Sismeing.Domain.Entities.Operaciones;
using Sismeing.Infrestructura.Persistence;
using Sismeing.Service.Interfaces.Comunes;
using Sismeing.Service.Interfaces.Operaciones;

namespace Sismeing.Service.Services.Operaciones
{
    public class FotoInstalacionService : IFoto_InstalacionService
    {
        private readonly SupaBaseDBcontext _context;
        private readonly IAuditoriaService _auditoriaService;

        public FotoInstalacionService(SupaBaseDBcontext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Foto_Instalacion>> GetAllAsync()
        {
            return await _context.FotosInstalacion.ToListAsync();
        }

        public async Task<Foto_Instalacion?> GetByIdAsync(int id)
        {
            return await _context.FotosInstalacion.FindAsync(id);
        }

        public async Task<IEnumerable<Foto_Instalacion>> GetByInstalacionIdAsync(int instalacionId)
        {
            return await _context.FotosInstalacion
                .Where(f => f.InstalacionId == instalacionId && f.Activo)
                .OrderBy(f => f.Tipo)
                .ThenBy(f => f.FechaRegistro)
                .ToListAsync();
        }

        public async Task<Foto_Instalacion> CreateAsync(Foto_Instalacion item, string usuarioRegistro)
        {
            item.Activo = true;
            item.UsuarioRegistro = usuarioRegistro;
            item.FechaRegistro = DateTime.UtcNow;
            item.IpRegistro = _auditoriaService.ObtenerIp();

            _context.FotosInstalacion.Add(item);
            await _context.SaveChangesAsync();

            return item;
        }

        public async Task<bool> UpdateAsync(int id, Foto_Instalacion item, string usuarioModificacion)
        {
            var existingItem = await _context.FotosInstalacion.FindAsync(id);
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
            var existingItem = await _context.FotosInstalacion.FindAsync(id);
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
