using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Sismeing.Domain.Entities.Operaciones;
using Sismeing.Domain.Models.Emails;
using Sismeing.Infrestructura.Persistence;
using Sismeing.Service.Interfaces.Comunes;
using Sismeing.Service.Interfaces.Operaciones;

namespace Sismeing.Service.Services.Operaciones
{
    public class NotificacionService : INotificacionService
    {
        // Roles con potestad para aprobar informes (Administrador, Supervisor,
        // SuperAdmin). Coincide con la politica "Aprobacion" de Program.cs: si
        // cambia una, debe cambiar la otra. Se comparan por ID y no por nombre
        // porque la etiqueta del rol es editable desde el panel de roles.
        private static readonly int[] RolesAprobadoresIds = { 30, 32, 34 };

        private readonly SupaBaseDBcontext _context;
        private readonly IAuditoriaService _auditoriaService;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _config;

        public NotificacionService(
            SupaBaseDBcontext context,
            IAuditoriaService auditoriaService,
            IEmailService emailService,
            IConfiguration config)
        {
            _context = context;
            _auditoriaService = auditoriaService;
            _emailService = emailService;
            _config = config;
        }

        // Mapea el nombre de un estado del catálogo al tipo de badge del frontend.
        // Catálogo actual: Pendiente, En progreso, Completado, Cancelado,
        // Requiere repuestos, Esperando aprobación.
        // "Esperando aprobación" se evalúa antes que "aprobado" a propósito.
        public static string TipoPorEstado(string nombreEstado)
        {
            var lower = nombreEstado.ToLowerInvariant();
            if (lower.Contains("esper") || lower.Contains("pend") || lower.Contains("revis") || lower.Contains("repuesto")) return "pendiente";
            if (lower.Contains("aprob")) return "aprobado";
            if (lower.Contains("complet") || lower.Contains("final")) return "completado";
            if (lower.Contains("program") || lower.Contains("proceso") || lower.Contains("progres")) return "programado";
            return "info";
        }

        public async Task<IEnumerable<Notificacion>> GetByUsuarioAsync(int usuarioId)
        {
            return await _context.Notificaciones
                .Where(n => n.UsuarioId == usuarioId && n.Activo)
                .OrderByDescending(n => n.FechaRegistro)
                .ToListAsync();
        }

        public async Task<int> GetNoLeidasCountAsync(int usuarioId)
        {
            return await _context.Notificaciones
                .CountAsync(n => n.UsuarioId == usuarioId && n.Activo && !n.Leida);
        }

        public async Task<Notificacion> CreateAsync(Notificacion item, string usuarioRegistro)
        {
            item.Activo = true;
            item.Leida = false;
            item.UsuarioRegistro = usuarioRegistro;
            item.FechaRegistro = DateTime.UtcNow;
            item.IpRegistro = _auditoriaService.ObtenerIp();

            _context.Notificaciones.Add(item);
            await _context.SaveChangesAsync();

            return item;
        }

        /// <summary>
        /// Avisa a los supervisores de que un informe quedo esperando aprobacion,
        /// por partida doble: notificacion dentro de la aplicacion y correo.
        /// Nunca lanza: un fallo de SMTP no puede tumbar el guardado del informe.
        /// </summary>
        public async Task NotificarPendienteAprobacionAsync(
            string tipoServicio, string origen, int referenciaId,
            string numeroInforme, string tecnico, string cliente, string usuarioRegistro)
        {
            List<Usuario> aprobadores;
            try
            {
                aprobadores = await _context.Usuarios
                    .Where(u => u.Activo && RolesAprobadoresIds.Contains(u.RolId))
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error buscando aprobadores: {ex.GetBaseException().Message}");
                return;
            }

            var frontendUrl = (_config["FrontendUrl"] ?? "http://localhost:5173").TrimEnd('/');
            var enlace = $"{frontendUrl}/informes/{origen}/{referenciaId}";

            foreach (var aprobador in aprobadores)
            {
                // Cada destinatario va en su propio try: si a uno le falla el correo,
                // los demas deben recibirlo igual.
                try
                {
                    await CreateAsync(new Notificacion
                    {
                        UsuarioId = aprobador.Id,
                        Titulo = "Informe pendiente de aprobación",
                        Mensaje = $"El informe {numeroInforme} de {tipoServicio} espera su aprobación.",
                        Tipo = "pendiente",
                        Origen = origen,
                        ReferenciaId = referenciaId,
                    }, usuarioRegistro);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error notificando aprobacion in-app a {aprobador.Id}: {ex.GetBaseException().Message}");
                }

                try
                {
                    if (string.IsNullOrWhiteSpace(aprobador.CorreoElectronico)) continue;

                    await _emailService.SendAsync(
                        aprobador.CorreoElectronico,
                        $"Informe {numeroInforme} pendiente de su aprobación",
                        "PendienteAprobacion",
                        new AprobacionPendienteModel
                        {
                            Supervisor = $"{aprobador.Nombre} {aprobador.Apellido}".Trim(),
                            TipoServicio = tipoServicio,
                            NumeroInforme = numeroInforme,
                            Tecnico = tecnico,
                            Cliente = cliente,
                            Enlace = enlace,
                        });
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error enviando correo de aprobacion a {aprobador.CorreoElectronico}: {ex.GetBaseException().Message}");
                }
            }
        }

        /// <summary>
        /// Avisa dentro de la aplicacion que un servicio recien creado espera
        /// revision: al supervisor asignado y a todo el que puede aprobar
        /// (administradores y superadmins), menos a quien lo creo. No lanza.
        /// </summary>
        public async Task NotificarNuevoServicioAsync(
            string tipoServicio, string origen, int referenciaId,
            string numeroInforme, int? supervisorId, string usuarioRegistro)
        {
            List<int> destinatarios;
            try
            {
                destinatarios = await _context.Usuarios
                    .Where(u => u.Activo
                        && RolesAprobadoresIds.Contains(u.RolId)
                        && u.CorreoElectronico != usuarioRegistro)
                    .Select(u => u.Id)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error buscando aprobadores: {ex.GetBaseException().Message}");
                return;
            }

            if (supervisorId.HasValue && !destinatarios.Contains(supervisorId.Value))
                destinatarios.Add(supervisorId.Value);

            foreach (var usuarioId in destinatarios)
            {
                try
                {
                    await CreateAsync(new Notificacion
                    {
                        UsuarioId = usuarioId,
                        Titulo = "Servicio Pendiente de Revisión",
                        Mensaje = $"El informe {numeroInforme} de {tipoServicio} requiere revisión y aprobación.",
                        Tipo = "pendiente",
                        Origen = origen,
                        ReferenciaId = referenciaId,
                    }, usuarioRegistro);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error notificando nuevo servicio a {usuarioId}: {ex.GetBaseException().Message}");
                }
            }
        }

        public async Task<bool> MarcarLeidaAsync(int id, string usuarioModificacion)
        {
            var existingItem = await _context.Notificaciones.FindAsync(id);
            if (existingItem == null) return false;

            existingItem.Leida = true;
            existingItem.FechaLeida = DateTime.UtcNow;
            existingItem.UsuarioModificacion = usuarioModificacion;
            existingItem.FechaModificacion = DateTime.UtcNow;
            existingItem.IpModificacion = _auditoriaService.ObtenerIp();

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<int> MarcarTodasLeidasAsync(int usuarioId, string usuarioModificacion)
        {
            var pendientes = await _context.Notificaciones
                .Where(n => n.UsuarioId == usuarioId && n.Activo && !n.Leida)
                .ToListAsync();

            var ahora = DateTime.UtcNow;
            var ip = _auditoriaService.ObtenerIp();
            foreach (var n in pendientes)
            {
                n.Leida = true;
                n.FechaLeida = ahora;
                n.UsuarioModificacion = usuarioModificacion;
                n.FechaModificacion = ahora;
                n.IpModificacion = ip;
            }

            await _context.SaveChangesAsync();
            return pendientes.Count;
        }

        public async Task<bool> DeleteAsync(int id, string usuarioEliminacion)
        {
            var existingItem = await _context.Notificaciones.FindAsync(id);
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
