using Microsoft.EntityFrameworkCore;
using Sismeing.Domain.Entities.Operaciones;
using Sismeing.Infrestructura.Persistence;
using Sismeing.Service.Interfaces.Comunes;
using Sismeing.Service.Interfaces.Operaciones;

namespace Sismeing.Service.Services.Operaciones
{
    public class VisitaTecnicaService : IVisita_TecnicaService
    {
        private readonly SupaBaseDBcontext _context;
        private readonly IAuditoriaService _auditoriaService;

        public VisitaTecnicaService(SupaBaseDBcontext context, IAuditoriaService auditoriaService)
        {
            _context = context;
            _auditoriaService = auditoriaService;
        }

        public async Task<IEnumerable<Visita_Tecnica>> GetAllAsync()
        {
            return await _context.VisitasTecnicas
                .Include(v => v.Empresa)
                .Include(v => v.Tecnico)
                .Include(v => v.TipoTrabajo)
                .ToListAsync();
        }

        public async Task<Visita_Tecnica?> GetByIdAsync(int id)
        {
            return await _context.VisitasTecnicas
                .Include(v => v.Empresa)
                .Include(v => v.Tecnico)
                .Include(v => v.TipoTrabajo)
                .FirstOrDefaultAsync(v => v.Id == id);
        }

        public async Task<Visita_Tecnica> CreateAsync(Visita_Tecnica item, string usuarioRegistro)
        {
            item.Activo = true;
            item.UsuarioRegistro = usuarioRegistro;
            item.FechaRegistro = DateTime.UtcNow;
            item.IpRegistro = _auditoriaService.ObtenerIp();
            
            _context.VisitasTecnicas.Add(item);
            await _context.SaveChangesAsync();

            return item;
        }

        public async Task<bool> UpdateAsync(int id, Visita_Tecnica item, string usuarioModificacion)
        {
            var existingItem = await _context.VisitasTecnicas.FindAsync(id);
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
            var existingItem = await _context.VisitasTecnicas.FindAsync(id);
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
