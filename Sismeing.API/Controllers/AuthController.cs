using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Sismeing.Domain.Entities.DTOs;
using Sismeing.Domain.Entities.Operaciones;
using Sismeing.Infrestructura.Persistence;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Sismeing.API.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly SupaBaseDBcontext _context;
        private readonly IConfiguration _configuration;

        public AuthController(SupaBaseDBcontext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        /// <summary>Login de usuario — retorna JWT.</summary>
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var usuario = await _context.Usuarios
                .Include(u => u.Rol)
                .Include(u => u.Empresa)
                .FirstOrDefaultAsync(u => u.CorreoElectronico == request.CorreoElectronico && u.Activo);

            if (usuario == null)
                return Unauthorized(new { mensaje = "Credenciales inválidas." });

            if (string.IsNullOrEmpty(usuario.PasswordHash))
                return Unauthorized(new { mensaje = "El usuario no tiene contraseña configurada. Contacte al administrador." });

            var passwordValido = BCrypt.Net.BCrypt.Verify(request.Password, usuario.PasswordHash);
            if (!passwordValido)
                return Unauthorized(new { mensaje = "Credenciales inválidas." });

            var token = GenerarToken(usuario);

            return Ok(new LoginResponseDto
            {
                Token = token.TokenString,
                Expiracion = token.Expiracion,
                Usuario = new UsuarioDto
                {
                    Id = usuario.Id,
                    Nombre = usuario.Nombre,
                    Apellido = usuario.Apellido,
                    CorreoElectronico = usuario.CorreoElectronico,
                    Telefono = usuario.Telefono,
                    NombreRol = usuario.Rol?.NombreRol ?? string.Empty,
                    RolId = usuario.RolId,
                    EmpresaId = usuario.EmpresaId,
                    NombreEmpresa = usuario.Empresa?.Nombre ?? string.Empty,
                    Verificado = usuario.Verificado
                }
            });
        }

        /// <summary>Registro de nuevo usuario (solo Admin).</summary>
        [Authorize]
        [HttpPost("register")]
        public async Task<ActionResult<UsuarioDto>> Register([FromBody] RegisterRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existe = await _context.Usuarios.AnyAsync(u =>
                u.CorreoElectronico == request.CorreoElectronico ||
                u.Cedula == request.Cedula);

            if (existe)
                return Conflict(new { mensaje = "Ya existe un usuario con ese correo o cédula." });

            var usuario = new Usuario
            {
                Nombre = request.Nombre,
                Apellido = request.Apellido,
                Cedula = request.Cedula,
                CorreoElectronico = request.CorreoElectronico,
                Telefono = request.Telefono,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                EmpresaId = request.EmpresaId,
                RolId = request.RolId,
                Verificado = false,
                FechaRegistro = DateTime.UtcNow,
                UsuarioRegistro = HttpContext.Items["UserEmail"]?.ToString() ?? "SYSTEM"
            };

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            var usuarioConRelaciones = await _context.Usuarios
                .Include(u => u.Rol)
                .Include(u => u.Empresa)
                .FirstAsync(u => u.Id == usuario.Id);

            return CreatedAtAction(nameof(Login), new UsuarioDto
            {
                Id = usuario.Id,
                Nombre = usuario.Nombre,
                Apellido = usuario.Apellido,
                CorreoElectronico = usuario.CorreoElectronico,
                Telefono = usuario.Telefono,
                NombreRol = usuarioConRelaciones.Rol?.NombreRol ?? string.Empty,
                RolId = usuario.RolId,
                EmpresaId = usuario.EmpresaId,
                NombreEmpresa = usuarioConRelaciones.Empresa?.Nombre ?? string.Empty,
                Verificado = usuario.Verificado
            });
        }

        // ── Generación del JWT ────────────────────────────────────────────────────

        private (string TokenString, DateTime Expiracion) GenerarToken(Usuario usuario)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var secretKey = Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]!);
            var expiresInHours = int.Parse(jwtSettings["ExpiresInHours"] ?? "8");
            var expiracion = DateTime.UtcNow.AddHours(expiresInHours);

            var claims = new[]
            {
                new Claim("id", usuario.Id.ToString()),
                new Claim("email", usuario.CorreoElectronico),
                new Claim("nombre", $"{usuario.Nombre} {usuario.Apellido}"),
                new Claim("rol", usuario.Rol?.NombreRol ?? string.Empty),
                new Claim("rol_id", usuario.RolId.ToString()),
                new Claim("empresa_id", usuario.EmpresaId.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Role, usuario.Rol?.NombreRol ?? string.Empty)
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = expiracion,
                Issuer = jwtSettings["Issuer"],
                Audience = jwtSettings["Audience"],
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(secretKey),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return (tokenHandler.WriteToken(token), expiracion);
        }
    }
}
