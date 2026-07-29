using Microsoft.EntityFrameworkCore;
using Sismeing.Domain.Entities.Operaciones;
using Sismeing.Infrestructura.Persistence;
using Sismeing.Service.Interfaces.Comunes;
using Sismeing.Service.Interfaces.Operaciones;

namespace Sismeing.Service.Services.Operaciones
{
    public class EquipoService : IEquipoService
    {
        private readonly SupaBaseDBcontext _context;
        private readonly IAuditoriaService _auditoriaService;
        private readonly IUsuarioContext _usuarioContext;

        public EquipoService(SupaBaseDBcontext context, IAuditoriaService auditoriaService, IUsuarioContext usuarioContext)
        {
            _context = context;
            _auditoriaService = auditoriaService;
            _usuarioContext = usuarioContext;
        }

        public async Task<IEnumerable<Equipo>> GetAllAsync()
        {
            var query = _context.Equipos
                .Include(e => e.Marca)
                .Include(e => e.TipoEquipo)
                .Include(e => e.Modelo)
                .Include(e => e.Proyecto)
                .Include(e => e.Area)
                .AsQueryable();

            // El Cliente solo ve los equipos de su empresa (por area o por proyecto).
            if (_usuarioContext.EsCliente && _usuarioContext.EmpresaId is int empId)
                query = query.Where(e =>
                    (e.Area != null && e.Area.EmpresaId == empId) ||
                    (e.Proyecto != null && e.Proyecto.EmpresaId == empId));

            return await query.ToListAsync();
        }

        public async Task<Equipo?> GetByIdAsync(int id)
        {
            return await _context.Equipos
                .Include(e => e.Marca)
                .Include(e => e.TipoEquipo)
                .Include(e => e.Modelo)
                .Include(e => e.Proyecto)
                .Include(e => e.Area)
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<Equipo> CreateAsync(Equipo item, string usuarioRegistro)
        {
            item.Activo = true;
            item.UsuarioRegistro = usuarioRegistro;
            item.FechaRegistro = DateTime.UtcNow;
            item.IpRegistro = _auditoriaService.ObtenerIp();

            _context.Equipos.Add(item);
            await _context.SaveChangesAsync();

            return item;
        }

        public async Task<bool> UpdateAsync(int id, Equipo item, string usuarioModificacion)
        {
            var existingItem = await _context.Equipos.FindAsync(id);
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
            var existingItem = await _context.Equipos.FindAsync(id);
            if (existingItem == null) return false;

            existingItem.Activo = false;
            existingItem.UsuarioEliminacion = usuarioEliminacion;
            existingItem.FechaEliminacion = DateTime.UtcNow;
            existingItem.IpEliminacion = _auditoriaService.ObtenerIp();

            await _context.SaveChangesAsync();
            return true;
        }

        // Reactiva un registro previamente desactivado (activo = true).
        public async Task<bool> ReactivarAsync(int id, string usuario)
        {
            var item = await _context.Equipos.FindAsync(id);
            if (item == null) return false;

            item.Activo = true;
            item.UsuarioModificacion = usuario;
            item.FechaModificacion = DateTime.UtcNow;
            item.IpModificacion = _auditoriaService.ObtenerIp();

            await _context.SaveChangesAsync();
            return true;
        }

    }
}