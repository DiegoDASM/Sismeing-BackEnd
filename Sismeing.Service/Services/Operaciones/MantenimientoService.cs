using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Sismeing.Domain.Entities.Operaciones;
using Sismeing.Domain.Enums;
using Sismeing.Infrestructura.Persistence;
using Sismeing.Service.Interfaces.Comunes;
using Sismeing.Service.Interfaces.Operaciones;
using Sismeing.Service.Services.Comunes;

namespace Sismeing.Service.Services.Operaciones
{
    public class MantenimientoService : IMantenimientoService
    {
        private readonly SupaBaseDBcontext _context;
        private readonly IAuditoriaService _auditoriaService;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly INotificacionService _notificacionService;

        public MantenimientoService(SupaBaseDBcontext context, IAuditoriaService auditoriaService, IServiceScopeFactory scopeFactory, INotificacionService notificacionService)
        {
            _context = context;
            _auditoriaService = auditoriaService;
            _scopeFactory = scopeFactory;
            _notificacionService = notificacionService;
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

        private static void NormalizarFechas(Mantenimiento item)
        {
            item.FechaInicio = EntityUpdateHelper.AsegurarUtc(item.FechaInicio);
            item.FechaFin = EntityUpdateHelper.AsegurarUtc(item.FechaFin);
            item.FechaProximo = EntityUpdateHelper.AsegurarUtc(item.FechaProximo);
        }

        public async Task<Mantenimiento> CreateAsync(Mantenimiento item, string usuarioRegistro)
        {
            NormalizarFechas(item);
            item.Activo = true;
            item.UsuarioRegistro = usuarioRegistro;
            item.FechaRegistro = DateTime.UtcNow;
            item.IpRegistro = _auditoriaService.ObtenerIp();

            _context.Mantenimientos.Add(item);
            await _context.SaveChangesAsync();

            // Notificación in-app: el supervisor debe revisar el nuevo servicio
            try
            {
                var informe = string.IsNullOrEmpty(item.NumeroInforme) ? $"#{item.Id}" : item.NumeroInforme;
                if (item.SupervisorId.HasValue)
                {
                    await _notificacionService.CreateAsync(new Notificacion
                    {
                        UsuarioId = item.SupervisorId.Value,
                        Titulo = "Servicio Pendiente de Revisión",
                        Mensaje = $"El mantenimiento {informe} requiere revisión y aprobación del supervisor.",
                        Tipo = "pendiente",
                        Origen = "mantenimiento",
                        ReferenciaId = item.Id,
                    }, usuarioRegistro);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creando notificación de mantenimiento: {ex.GetBaseException().Message}");
            }

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

            var estadoAnteriorId = existingItem.EstadoId;

            NormalizarFechas(item);
            var entry = _context.Entry(existingItem);
            entry.CurrentValues.SetValues(item);
            EntityUpdateHelper.PreservarCamposRegistro(entry);
            existingItem.UsuarioModificacion = usuarioModificacion;
            existingItem.FechaModificacion = DateTime.UtcNow;
            existingItem.IpModificacion = _auditoriaService.ObtenerIp();

            await _context.SaveChangesAsync();

            // Notificación in-app al técnico (y encargado) cuando cambia el estado
            if (existingItem.EstadoId != estadoAnteriorId)
            {
                try
                {
                    var estado = await _context.Estados.FindAsync(existingItem.EstadoId);
                    var nombreEstado = estado?.NombreEstado ?? "Actualizado";
                    var tipo = NotificacionService.TipoPorEstado(nombreEstado);
                    var informe = string.IsNullOrEmpty(existingItem.NumeroInforme) ? $"#{existingItem.Id}" : existingItem.NumeroInforme;

                    var destinatarios = new List<int> { existingItem.TecnicoId };
                    if (existingItem.EncargadoId.HasValue && existingItem.EncargadoId.Value != existingItem.TecnicoId)
                        destinatarios.Add(existingItem.EncargadoId.Value);

                    foreach (var usuarioId in destinatarios)
                    {
                        await _notificacionService.CreateAsync(new Notificacion
                        {
                            UsuarioId = usuarioId,
                            Titulo = $"Mantenimiento {nombreEstado}",
                            Mensaje = $"El mantenimiento {informe} cambió de estado a \"{nombreEstado}\".",
                            Tipo = tipo,
                            Origen = "mantenimiento",
                            ReferenciaId = existingItem.Id,
                        }, usuarioModificacion);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error creando notificación de mantenimiento: {ex.GetBaseException().Message}");
                }
            }

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
