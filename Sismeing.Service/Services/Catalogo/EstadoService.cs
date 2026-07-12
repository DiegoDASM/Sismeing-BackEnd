using Microsoft.EntityFrameworkCore;
using Sismeing.Domain.Entities.Catalogo;
using Sismeing.Infrestructura.Persistence;
using Sismeing.Service.Interfaces.Catalogo;
using Sismeing.Service.Interfaces.Comunes;
using Sismeing.Service.Services.Comunes;

namespace Sismeing.Service.Services.Catalogo
{
    public class EstadoService : IEstadoService
    {
        private readonly SupaBaseDBcontext _context;
        private readonly IAuditoriaService _auditoriaService;

        public EstadoService(SupaBaseDBcontext context, IAuditoriaService auditoriaService)
        {
            _context = context;
            _auditoriaService = auditoriaService;
        }

        public async Task<IEnumerable<Estado>> GetAllAsync()
        {
            return await _context.Estados.ToListAsync();
        }

        public async Task<Estado?> GetByIdAsync(int id)
        {
            return await _context.Estados.FindAsync(id);
        }

        public async Task<Estado> CreateAsync(Estado item, string usuarioRegistro)
        {
            // Unicidad: no registrar el mismo nombre dos veces (case-insensitive)
            var nombreNuevo = (item.NombreEstado ?? string.Empty).Trim();
            if (nombreNuevo.Length == 0)
                throw new InvalidOperationException("El nombre es obligatorio.");
            if (await _context.Estados.AnyAsync(x => x.Activo && x.NombreEstado.ToLower() == nombreNuevo.ToLower()))
                throw new InvalidOperationException($"Ya existe un estado con el nombre '{nombreNuevo}'.");
            item.NombreEstado = nombreNuevo;

            item.UsuarioRegistro = usuarioRegistro;
            item.FechaRegistro = DateTime.UtcNow;
            item.Activo = true;
            item.IpRegistro = _auditoriaService.ObtenerIp();

            _context.Estados.Add(item);
            await _context.SaveChangesAsync();

            return item;
        }

        public async Task<bool> UpdateAsync(int id, Estado item, string usuarioModificacion)
        {
            var existingItem = await _context.Estados.FindAsync(id);
            if (existingItem == null) return false;

            var nombreNuevo = (item.NombreEstado ?? string.Empty).Trim();
            if (await _context.Estados.AnyAsync(x => x.Id != id && x.Activo && x.NombreEstado.ToLower() == nombreNuevo.ToLower()))
                throw new InvalidOperationException($"Ya existe un estado con el nombre '{nombreNuevo}'.");
            item.NombreEstado = nombreNuevo;

            var entry = _context.Entry(existingItem);
            entry.CurrentValues.SetValues(item);
            EntityUpdateHelper.PreservarCamposRegistro(entry);
            existingItem.UsuarioModificacion = usuarioModificacion;
            existingItem.FechaModificacion = DateTime.UtcNow;
            existingItem.IpModificacion = _auditoriaService.ObtenerIp();
            
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id, string usuarioEliminacion)
        {
            var existingItem = await _context.Estados.FindAsync(id);
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