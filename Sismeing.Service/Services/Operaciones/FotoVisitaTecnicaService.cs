using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Sismeing.Domain.Entities.Operaciones;
using Sismeing.Infrestructura.Persistence;
using Sismeing.Service.Interfaces.Comunes;
using Sismeing.Service.Interfaces.Operaciones;

namespace Sismeing.Service.Services.Operaciones
{
    public class FotoVisitaTecnicaService : IFoto_VisitaTecnicaService
    {
        private readonly SupaBaseDBcontext _context;
        private readonly IAuditoriaService _auditoriaService;

        public FotoVisitaTecnicaService(SupaBaseDBcontext context, IAuditoriaService auditoriaService)
        {
            _context = context;
            _auditoriaService = auditoriaService;
        }

        public async Task<IEnumerable<Foto_VisitaTecnica>> GetAllAsync()
        {
            return await _context.FotosVisitaTecnica.Where(f => f.Activo).ToListAsync();
        }

        public async Task<Foto_VisitaTecnica?> GetByIdAsync(int id)
        {
            return await _context.FotosVisitaTecnica.FirstOrDefaultAsync(f => f.Id == id && f.Activo);
        }

        public async Task<IEnumerable<Foto_VisitaTecnica>> GetByVisitaTecnicaIdAsync(int visitaTecnicaId)
        {
            return await _context.FotosVisitaTecnica
                .Where(f => f.VisitaTecnicaId == visitaTecnicaId && f.Activo)
                .ToListAsync();
        }

        public async Task<Foto_VisitaTecnica> CreateAsync(Foto_VisitaTecnica item, string usuarioRegistro)
        {
            item.Activo = true;
            item.UsuarioRegistro = usuarioRegistro;
            item.FechaRegistro = DateTime.UtcNow;
            item.IpRegistro = _auditoriaService.ObtenerIp();

            _context.FotosVisitaTecnica.Add(item);
            await _context.SaveChangesAsync();

            return item;
        }

        public async Task<bool> DeleteAsync(int id, string usuarioEliminacion)
        {
            var existingItem = await _context.FotosVisitaTecnica.FindAsync(id);
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
