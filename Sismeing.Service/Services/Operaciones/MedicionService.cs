using Microsoft.EntityFrameworkCore;
using Sismeing.Domain.Entities.Operaciones;
using Sismeing.Infrestructura.Persistence;
using Sismeing.Service.Interfaces.Comunes;
using Sismeing.Service.Interfaces.Operaciones;

namespace Sismeing.Service.Services.Operaciones
{
    public class MedicionService : IMedicionService
    {
        private readonly SupaBaseDBcontext _context;
        private readonly IAuditoriaService _auditoriaService;

        public MedicionService(SupaBaseDBcontext context, IAuditoriaService auditoriaService)
        {
            _context = context;
            _auditoriaService = auditoriaService;
        }

        public async Task<IEnumerable<Medicion>> GetAllAsync()
        {
            return await _context.Mediciones.ToListAsync();
        }

        public async Task<Medicion?> GetByIdAsync(int id)
        {
            return await _context.Mediciones.FindAsync(id);
        }

        public async Task<IEnumerable<Medicion>> GetByInformeAsync(int informeId, int equipoId)
        {
            return await _context.Mediciones
                .Where(m => m.InformeId == informeId && m.EquipoId == equipoId && m.Activo)
                .OrderByDescending(m => m.Inicial)
                .ToListAsync();
        }

        public async Task<Medicion> CreateAsync(Medicion item, string usuarioRegistro)
        {
            item.Activo = true;
            item.UsuarioRegistro = usuarioRegistro;
            item.FechaRegistro = DateTime.UtcNow;
            item.IpRegistro = _auditoriaService.ObtenerIp();

            _context.Mediciones.Add(item);
            await _context.SaveChangesAsync();

            return item;
        }

        public async Task<IEnumerable<Medicion>> CreateBatchAsync(IEnumerable<Medicion> items, string usuarioRegistro)
        {
            var now = DateTime.UtcNow;
            var ip = _auditoriaService.ObtenerIp();
            var lista = items.ToList();

            foreach (var item in lista)
            {
                item.Id = 0;
                item.Activo = true;
                item.UsuarioRegistro = usuarioRegistro;
                item.FechaRegistro = now;
                item.IpRegistro = ip;
            }

            _context.Mediciones.AddRange(lista);
            await _context.SaveChangesAsync();
            return lista;
        }

        /// <summary>
        /// Reemplaza todas las mediciones (inicial y final) de un informe+equipo:
        /// da de baja las anteriores y guarda las nuevas. Sirve para crear y editar.
        /// </summary>
        public async Task<IEnumerable<Medicion>> ReemplazarPorInformeAsync(int informeId, int equipoId, IEnumerable<Medicion> items, string usuarioRegistro)
        {
            var now = DateTime.UtcNow;
            var ip = _auditoriaService.ObtenerIp();

            var anteriores = await _context.Mediciones
                .Where(m => m.InformeId == informeId && m.EquipoId == equipoId && m.Activo)
                .ToListAsync();
            foreach (var ant in anteriores)
            {
                ant.Activo = false;
                ant.UsuarioEliminacion = usuarioRegistro;
                ant.FechaEliminacion = now;
                ant.IpEliminacion = ip;
            }

            var nuevos = items.ToList();
            foreach (var item in nuevos)
            {
                item.Id = 0;
                item.InformeId = informeId;
                item.EquipoId = equipoId;
                item.Activo = true;
                item.UsuarioRegistro = usuarioRegistro;
                item.FechaRegistro = now;
                item.IpRegistro = ip;
            }

            _context.Mediciones.AddRange(nuevos);
            await _context.SaveChangesAsync();
            return nuevos;
        }

        public async Task<bool> UpdateAsync(int id, Medicion item, string usuarioModificacion)
        {
            var existingItem = await _context.Mediciones.FindAsync(id);
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
            var existingItem = await _context.Mediciones.FindAsync(id);
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