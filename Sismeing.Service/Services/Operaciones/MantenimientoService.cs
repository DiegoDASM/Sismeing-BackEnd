using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Sismeing.Domain.Entities.Operaciones;
using Sismeing.Domain.Enums;
using Sismeing.Infrestructura.Persistence;
using Sismeing.Service.Interfaces.Comunes;
using Sismeing.Service.Interfaces.Operaciones;

namespace Sismeing.Service.Services.Operaciones
{
    public class MantenimientoService : IMantenimientoService
    {
        private readonly SupaBaseDBcontext _context;
        private readonly IAuditoriaService _auditoriaService;
        private readonly IServiceScopeFactory _scopeFactory;

        public MantenimientoService(SupaBaseDBcontext context, IAuditoriaService auditoriaService, IServiceScopeFactory scopeFactory)
        {
            _context = context;
            _auditoriaService = auditoriaService;
            _scopeFactory = scopeFactory;
        }

        public async Task<IEnumerable<Mantenimiento>> GetAllAsync()
        {
            return await _context.Mantenimientos
                .Include(m => m.Instalacion)
                    .ThenInclude(i => i.Equipo)
                .Include(m => m.Instalacion)
                    .ThenInclude(i => i.Area).ThenInclude(a => a.Empresa)
                .Include(m => m.TipoMantenimiento)
                .Include(m => m.Tecnico)
                .Include(m => m.Estado)
                .ToListAsync();
        }

        public async Task<Mantenimiento?> GetByIdAsync(int id)
        {
            return await _context.Mantenimientos
                .Include(m => m.Instalacion)
                    .ThenInclude(i => i.Equipo)
                .Include(m => m.Instalacion)
                    .ThenInclude(i => i.Area).ThenInclude(a => a.Empresa)
                .Include(m => m.TipoMantenimiento)
                .Include(m => m.Tecnico)
                .Include(m => m.Estado)
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<Mantenimiento> CreateAsync(Mantenimiento item, string usuarioRegistro)
        {
            item.Activo = true;
            item.UsuarioRegistro = usuarioRegistro;
            item.FechaRegistro = DateTime.UtcNow;
            item.IpRegistro = _auditoriaService.ObtenerIp();

            _context.Mantenimientos.Add(item);
            await _context.SaveChangesAsync();

            if (item.EnviarCorreoRecordatorio)
            {
                _ = Task.Run(async () =>
                {
                    try
                    {
                        using var scope = _scopeFactory.CreateScope();
                        var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();
                        var dbContext = scope.ServiceProvider.GetRequiredService<SupaBaseDBcontext>();
                        
                        // Por ahora lo enviamos al encargado o al técnico.
                        // Esto se puede cambiar si necesitas enviárselo a la Empresa.
                        var targetUserId = item.EncargadoId ?? item.TecnicoId;
                        var usuarioDestino = await dbContext.Usuarios.FindAsync(targetUserId);

                        if (usuarioDestino != null)
                        {
                            await emailService.EnviarCorreoPredefinidoAsync(TipoCorreo.RecordatorioMantenimiento, usuarioDestino);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error programando correo recordatorio: {ex.Message}");
                    }
                });
            }

            return item;
        }

        public async Task<bool> UpdateAsync(int id, Mantenimiento item, string usuarioModificacion)
        {
            var existingItem = await _context.Mantenimientos.FindAsync(id);
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
            var existingItem = await _context.Mantenimientos.FindAsync(id);
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