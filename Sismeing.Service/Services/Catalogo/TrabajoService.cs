using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Sismeing.Domain.Entities.Catalogo;
using Sismeing.Infrestructura.Persistence;
using Sismeing.Service.Interfaces.Catalogo;
using Sismeing.Service.Interfaces.Comunes;

namespace Sismeing.Service.Services.Catalogo
{
    public class TrabajoService : ITrabajoService
    {
        private readonly SupaBaseDBcontext _context;
        private readonly IAuditoriaService _auditoriaService;

        public TrabajoService(SupaBaseDBcontext context, IAuditoriaService auditoriaService)
        {
            _context = context;
            _auditoriaService = auditoriaService;
        }

        public async Task<IEnumerable<Trabajo>> GetAllAsync()
        {
            return await _context.Trabajos.ToListAsync();
        }

        public async Task<Trabajo?> GetByIdAsync(int id)
        {
            return await _context.Trabajos.FindAsync(id);
        }

        public async Task<IEnumerable<Trabajo>> GetByMantenimientoAsync(int mantenimientoId)
        {
            return await _context.Trabajos
                .Where(t => t.MantenimientoId == mantenimientoId && t.Activo)
                .OrderBy(t => t.Id)
                .ToListAsync();
        }

        public async Task<IEnumerable<Trabajo>> GetByInstalacionAsync(int instalacionId)
        {
            return await _context.Trabajos
                .Where(t => t.InstalacionId == instalacionId && t.Activo)
                .OrderBy(t => t.Id)
                .ToListAsync();
        }

        public async Task<Trabajo> CreateAsync(Trabajo item, string usuarioRegistro)
        {
            // No permitir trabajos de catálogo repetidos (sin distinguir mayúsculas).
            // Solo se valida contra los "templates" del catálogo (sin servicio asociado).
            if (item.MantenimientoId == null && item.InstalacionId == null)
            {
                var nombre = (item.NombreTrabajo ?? "").Trim().ToLower();
                var existe = await _context.Trabajos
                    .AnyAsync(t => t.Activo && t.MantenimientoId == null && t.InstalacionId == null
                        && t.NombreTrabajo.ToLower() == nombre);
                if (existe)
                    throw new InvalidOperationException($"Ya existe el trabajo '{item.NombreTrabajo}'.");
            }

            item.Activo = true;
            item.UsuarioRegistro = usuarioRegistro;
            item.FechaRegistro = DateTime.UtcNow;
            item.IpRegistro = _auditoriaService.ObtenerIp();

            _context.Trabajos.Add(item);
            await _context.SaveChangesAsync();

            return item;
        }

        public Task<IEnumerable<Trabajo>> ReemplazarPorMantenimientoAsync(int mantenimientoId, IEnumerable<Trabajo> items, string usuarioRegistro) =>
            ReemplazarAsync(t => t.MantenimientoId == mantenimientoId, t => t.MantenimientoId = mantenimientoId, items, usuarioRegistro);

        public Task<IEnumerable<Trabajo>> ReemplazarPorInstalacionAsync(int instalacionId, IEnumerable<Trabajo> items, string usuarioRegistro) =>
            ReemplazarAsync(t => t.InstalacionId == instalacionId, t => t.InstalacionId = instalacionId, items, usuarioRegistro);

        /// <summary>
        /// Sincroniza los trabajos realizados de un servicio con la lista recibida.
        /// Sirve para crear y editar. Cuando el nombre coincide se CONSERVA la fila
        /// (mismo id): las fotos que apuntan al trabajo por trabajo_id siguen
        /// agrupadas tras una edición. Solo se da de baja lo quitado y se agrega lo nuevo.
        /// </summary>
        private async Task<IEnumerable<Trabajo>> ReemplazarAsync(
            Expression<Func<Trabajo, bool>> delServicio,
            Action<Trabajo> vincularServicio,
            IEnumerable<Trabajo> items, string usuarioRegistro)
        {
            var now = DateTime.UtcNow;
            var ip = _auditoriaService.ObtenerIp();

            var actuales = await _context.Trabajos
                .Where(delServicio).Where(t => t.Activo)
                .ToListAsync();

            static string Clave(string? nombre) => (nombre ?? "").Trim().ToLower();

            var vivos = new List<Trabajo>();
            // Nombres ya procesados: un nombre repetido en la lista entrante se
            // ignora en vez de crear un trabajo duplicado en el servicio.
            var vistos = new HashSet<string>();
            foreach (var item in items.Where(t => !string.IsNullOrWhiteSpace(t.NombreTrabajo)))
            {
                if (!vistos.Add(Clave(item.NombreTrabajo))) continue;

                var existente = actuales.FirstOrDefault(a =>
                    !vivos.Contains(a) && Clave(a.NombreTrabajo) == Clave(item.NombreTrabajo));

                if (existente != null)
                {
                    if ((existente.Descripcion ?? "") != (item.Descripcion ?? ""))
                    {
                        existente.Descripcion = item.Descripcion;
                        existente.UsuarioModificacion = usuarioRegistro;
                        existente.FechaModificacion = now;
                        existente.IpModificacion = ip;
                    }
                    vivos.Add(existente);
                    continue;
                }

                var nuevo = new Trabajo
                {
                    NombreTrabajo = item.NombreTrabajo.Trim(),
                    Descripcion = item.Descripcion,
                    Activo = true,
                    UsuarioRegistro = usuarioRegistro,
                    FechaRegistro = now,
                    IpRegistro = ip,
                };
                vincularServicio(nuevo);
                _context.Trabajos.Add(nuevo);
                vivos.Add(nuevo);
            }

            foreach (var sobrante in actuales.Where(a => !vivos.Contains(a)))
            {
                sobrante.Activo = false;
                sobrante.UsuarioEliminacion = usuarioRegistro;
                sobrante.FechaEliminacion = now;
                sobrante.IpEliminacion = ip;
            }

            await _context.SaveChangesAsync();
            return vivos.OrderBy(t => t.Id).ToList();
        }

        public async Task<bool> UpdateAsync(int id, Trabajo item, string usuarioModificacion)
        {
            var existingItem = await _context.Trabajos.FindAsync(id);
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
            var existingItem = await _context.Trabajos.FindAsync(id);
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