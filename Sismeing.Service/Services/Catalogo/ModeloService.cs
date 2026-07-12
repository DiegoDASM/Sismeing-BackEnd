using Microsoft.EntityFrameworkCore;
using Sismeing.Domain.Entities.Catalogo;
using Sismeing.Infrestructura.Persistence;
using Sismeing.Service.Interfaces.Catalogo;
using Sismeing.Service.Interfaces.Comunes;
using Sismeing.Service.Services.Comunes;

namespace Sismeing.Service.Services.Catalogo
{
    public class ModeloService : IModeloService
    {
        private readonly SupaBaseDBcontext _context;
        private readonly IAuditoriaService _auditoriaService;

        public ModeloService(SupaBaseDBcontext context, IAuditoriaService auditoriaService)
        {
            _context = context;
            _auditoriaService = auditoriaService;
        }

        public async Task<IEnumerable<Modelo>> GetAllAsync()
        {
            return await _context.Modelos.ToListAsync();
        }

        public async Task<Modelo?> GetByIdAsync(int id)
        {
            return await _context.Modelos.FindAsync(id);
        }

        public async Task<Modelo> CreateAsync(Modelo item, string usuarioRegistro)
        {
            // Unicidad: no registrar el mismo nombre dos veces (case-insensitive)
            var nombreNuevo = (item.Nombre ?? string.Empty).Trim();
            if (nombreNuevo.Length == 0)
                throw new InvalidOperationException("El nombre es obligatorio.");
            if (await _context.Modelos.AnyAsync(x => x.Activo && x.Nombre.ToLower() == nombreNuevo.ToLower()))
                throw new InvalidOperationException($"Ya existe un modelo con el nombre '{nombreNuevo}'.");
            item.Nombre = nombreNuevo;

            item.UsuarioRegistro = usuarioRegistro;
            item.FechaRegistro = DateTime.UtcNow;
            item.Activo = true;
            item.IpRegistro = _auditoriaService.ObtenerIp();

            _context.Modelos.Add(item);
            await _context.SaveChangesAsync();

            return item;
        }

        public async Task<bool> UpdateAsync(int id, Modelo item, string usuarioModificacion)
        {
            var existingItem = await _context.Modelos.FindAsync(id);
            if (existingItem == null) return false;

            var nombreNuevo = (item.Nombre ?? string.Empty).Trim();
            if (await _context.Modelos.AnyAsync(x => x.Id != id && x.Activo && x.Nombre.ToLower() == nombreNuevo.ToLower()))
                throw new InvalidOperationException($"Ya existe un modelo con el nombre '{nombreNuevo}'.");
            item.Nombre = nombreNuevo;

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
            var existingItem = await _context.Modelos.FindAsync(id);
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