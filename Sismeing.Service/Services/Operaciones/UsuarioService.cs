using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Sismeing.Domain.Entities.Operaciones;
using Sismeing.Domain.Enums;
using Sismeing.Infrestructura.Persistence;
using Sismeing.Service.Interfaces.Comunes;
using Sismeing.Service.Services.Comunes;
using Sismeing.Service.Interfaces.Operaciones;

namespace Sismeing.Service.Services.Operaciones
{
    public class UsuarioService : IUsuarioService
    {
        private readonly SupaBaseDBcontext _context;
        private readonly IPasswordHasher<Usuario> _passwordHasher;
        private readonly IAuditoriaService _auditoriaService;
        private readonly IServiceScopeFactory _scopeFactory;

        public UsuarioService(SupaBaseDBcontext context, IPasswordHasher<Usuario> passwordHasher, IAuditoriaService auditoriaService, IServiceScopeFactory scopeFactory)
        {
            _context = context;
            _passwordHasher = passwordHasher;
            _auditoriaService = auditoriaService;
            _scopeFactory = scopeFactory;
        }

        public async Task<IEnumerable<Usuario>> GetAllAsync()
        {
            return await _context.Usuarios.Where(u => u.Activo).ToListAsync();
        }

        // Para el panel de cuentas: TODOS los usuarios (activos e inactivos)
        // con su rol y empresa.
        public async Task<IEnumerable<Usuario>> GetTodosAsync()
        {
            return await _context.Usuarios
                .Include(u => u.Rol)
                .Include(u => u.Empresa)
                .OrderBy(u => u.RolId).ThenBy(u => u.Nombre)
                .ToListAsync();
        }

        // Reactiva una cuenta desactivada.
        public async Task<bool> ReactivarAsync(int id, string usuario)
        {
            var item = await _context.Usuarios.FindAsync(id);
            if (item == null) return false;

            item.Activo = true;
            item.UsuarioModificacion = usuario;
            item.FechaModificacion = DateTime.UtcNow;
            item.IpModificacion = _auditoriaService.ObtenerIp();

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Usuario?> GetByIdAsync(int id)
        {
            return await _context.Usuarios.FindAsync(id);
        }

        public async Task<Usuario> CreateAsync(Usuario item, string usuarioRegistro)
        {

            if (!string.IsNullOrEmpty(item.Contrasena))
            {
                item.Contrasena = _passwordHasher.HashPassword(item, item.Contrasena);
            }

            item.Activo = true;
            item.UsuarioRegistro = usuarioRegistro;
            item.FechaRegistro = DateTime.UtcNow;
            item.IpRegistro = _auditoriaService.ObtenerIp();

            _context.Usuarios.Add(item);
            await _context.SaveChangesAsync();

            // Ejecutamos el envío de correo en un hilo en segundo plano (Fire and Forget)
            // para no demorar la respuesta de la API al cliente.
            _ = Task.Run(async () =>
            {
                try
                {
                    using var scope = _scopeFactory.CreateScope();
                    var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();
                    await emailService.EnviarCorreoPredefinidoAsync(TipoCorreo.Bienvenida, item);
                }
                catch (Exception ex)
                {
                    // Error capturado en el hilo de fondo
                    Console.WriteLine($"Error enviando correo en background: {ex.Message}");
                }
            });

            return item;
        }

        public async Task<bool> UpdateAsync(int id, Usuario item, string usuarioModificacion)
        {
            var existingItem = await _context.Usuarios.FindAsync(id);
            if (existingItem == null) return false;

            var entry = _context.Entry(existingItem);
            entry.CurrentValues.SetValues(item);
            EntityUpdateHelper.PreservarCamposRegistro(entry);

            // Contrasena e InvitacionToken llevan [JsonIgnore]: nunca se deserializan
            // del cuerpo de la peticion, asi que SetValues los dejaria en null y la
            // cuenta quedaria sin poder iniciar sesion. La contrasena solo se cambia
            // por CompletarRegistroAsync o el endpoint cambiar-contrasena.
            EntityUpdateHelper.PreservarSiVacio(entry, "Contrasena", "InvitacionToken");

            // El formulario de edicion no envia Activo (UpdateUsuarioDto lo omite),
            // asi que llega como false y desactivaria la cuenta. Activar/desactivar
            // tiene sus propios endpoints (ReactivarAsync / DeleteAsync).
            entry.Property(u => u.Activo).IsModified = false;

            existingItem.UsuarioModificacion = usuarioModificacion;
            existingItem.FechaModificacion = DateTime.UtcNow;
            existingItem.IpModificacion = _auditoriaService.ObtenerIp();

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id, string usuarioEliminacion)
        {
            var existingItem = await _context.Usuarios.FindAsync(id);
            if (existingItem == null) return false;

            existingItem.Activo = false;
            existingItem.UsuarioEliminacion = usuarioEliminacion;
            existingItem.FechaEliminacion = DateTime.UtcNow;
            existingItem.IpEliminacion = _auditoriaService.ObtenerIp();

            await _context.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Cambia el rol de un usuario (atribución del SuperAdmin). El nuevo rol
        /// aplica cuando el usuario vuelva a iniciar sesión: el token vigente
        /// conserva el claim con el que se emitió.
        /// </summary>
        public async Task<bool> CambiarRolAsync(int usuarioId, int rolId, string usuarioModificacion)
        {
            var usuario = await _context.Usuarios.FindAsync(usuarioId);
            if (usuario == null) return false;

            var rol = await _context.Roles.FirstOrDefaultAsync(r => r.Id == rolId && r.Activo)
                ?? throw new InvalidOperationException("El rol seleccionado no existe o está inactivo.");

            usuario.RolId = rol.Id;
            usuario.UsuarioModificacion = usuarioModificacion;
            usuario.FechaModificacion = DateTime.UtcNow;
            usuario.IpModificacion = _auditoriaService.ObtenerIp();

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdatePerfilAsync(int id, string nombre, string apellido, string? telefono, string userEmail)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null) return false;

            usuario.Nombre = nombre;
            usuario.Apellido = apellido;
            usuario.Telefono = telefono;
            usuario.UsuarioModificacion = userEmail;
            usuario.FechaModificacion = DateTime.UtcNow;
            usuario.IpModificacion = _auditoriaService.ObtenerIp();

            await _context.SaveChangesAsync();
            return true;
        }

        // ── Invitación de usuario ─────────────────────────────────────────────
        // Crea (o reutiliza) un usuario "pendiente" con solo el correo y le envía
        // un correo con el enlace para completar su registro.
        // rolId opcional: sin él se asume "Cliente" (invitación de encargado
        // desde la ficha del cliente, que no elige rol).
        public async Task InvitarUsuarioAsync(string correo, int? rolId, int empresaId, string usuarioRegistro)
        {
            correo = (correo ?? string.Empty).Trim().ToLower();
            if (string.IsNullOrWhiteSpace(correo))
                throw new InvalidOperationException("El correo es obligatorio.");

            var rol = rolId.HasValue
                ? await _context.Roles.FirstOrDefaultAsync(r => r.Id == rolId.Value && r.Activo)
                    ?? throw new InvalidOperationException("El rol seleccionado no existe o está inactivo.")
                : await _context.Roles.FirstOrDefaultAsync(r => r.NombreRol == "Cliente" && r.Activo)
                    ?? throw new InvalidOperationException("No existe el rol 'Cliente' en el catálogo.");

            var token = Guid.NewGuid().ToString("N");
            var existente = await _context.Usuarios.FirstOrDefaultAsync(u => u.CorreoElectronico.ToLower() == correo);

            Usuario usuario;
            if (existente != null)
            {
                if (existente.Verificado && existente.Activo)
                    throw new InvalidOperationException("Ya existe un usuario registrado con ese correo.");

                // Reinvitar: renovar token y reasociar a la empresa.
                existente.InvitacionToken = token;
                existente.EmpresaId = empresaId;
                existente.RolId = rol.Id;
                existente.Activo = true;
                existente.UsuarioModificacion = usuarioRegistro;
                existente.FechaModificacion = DateTime.UtcNow;
                usuario = existente;
            }
            else
            {
                usuario = new Usuario
                {
                    CorreoElectronico = correo,
                    Nombre = "",
                    Apellido = "",
                    Cedula = $"PEND-{token[..12]}",   // placeholder único (cedula es UNIQUE)
                    Contrasena = "PENDIENTE",          // se reemplaza al completar el registro
                    Verificado = false,
                    EmpresaId = empresaId,
                    RolId = rol.Id,
                    InvitacionToken = token,
                    Activo = true,
                    UsuarioRegistro = usuarioRegistro,
                    FechaRegistro = DateTime.UtcNow,
                    IpRegistro = _auditoriaService.ObtenerIp(),
                };
                _context.Usuarios.Add(usuario);
            }

            await _context.SaveChangesAsync();

            // Cargar la empresa para el nombre en el correo.
            usuario.Empresa = await _context.Empresas.FindAsync(empresaId);

            using var scope = _scopeFactory.CreateScope();
            var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();
            await emailService.EnviarCorreoPredefinidoAsync(TipoCorreo.InvitacionEncargado, usuario, token);
        }

        public async Task<(string correo, string empresaNombre)?> GetInvitacionAsync(string token)
        {
            var usuario = await _context.Usuarios
                .Include(u => u.Empresa)
                .FirstOrDefaultAsync(u => u.InvitacionToken == token);
            if (usuario == null) return null;
            return (usuario.CorreoElectronico, usuario.Empresa?.Nombre ?? "");
        }

        public async Task<bool> CompletarRegistroAsync(string token, string nombre, string apellido, string cedula, string contrasena)
        {
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.InvitacionToken == token);
            if (usuario == null) return false;

            cedula = (cedula ?? string.Empty).Trim();
            var cedulaEnUso = await _context.Usuarios.AnyAsync(u => u.Cedula == cedula && u.Id != usuario.Id);
            if (cedulaEnUso)
                throw new InvalidOperationException("La cédula ya está registrada por otro usuario.");

            usuario.Nombre = (nombre ?? "").Trim();
            usuario.Apellido = (apellido ?? "").Trim();
            usuario.Cedula = cedula;
            usuario.Contrasena = _passwordHasher.HashPassword(usuario, contrasena);
            usuario.Verificado = true;
            usuario.InvitacionToken = null;
            usuario.FechaModificacion = DateTime.UtcNow;
            usuario.IpModificacion = _auditoriaService.ObtenerIp();

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<string?> UpdateFotoAsync(int id, string fotoUrl)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null) return null;


            usuario.FechaModificacion = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return fotoUrl;
        }
    }
}