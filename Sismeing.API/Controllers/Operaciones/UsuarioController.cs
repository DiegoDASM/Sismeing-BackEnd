using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Sismeing.Domain.Entities.Operaciones;
using Sismeing.Infrestructura.Persistence;
using Sismeing.Service;
using Sismeing.Service.EntitiesDTO;
using Sismeing.Domain.Entities.DTOs;
using Sismeing.Service.Interfaces.Operaciones;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Sismeing.API.Controllers.Operaciones
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class UsuarioController : ControllerBase
    {
        private readonly IUsuarioService _usuarioService;
        private readonly SupaBaseDBcontext _context;
        private readonly IPasswordHasher<Usuario> _passwordHasher;
        private readonly IConfiguration _configuration;

        public UsuarioController(
            IUsuarioService usuarioService,
            SupaBaseDBcontext context,
            IPasswordHasher<Usuario> passwordHasher,
            IConfiguration configuration)
        {
            _usuarioService = usuarioService;
            _context = context;
            _passwordHasher = passwordHasher;
            _configuration = configuration;
        }

        [HttpGet]
        public async Task<ActionResult> GetAll()
        {
            try
            {
                var data = await _usuarioService.GetAllAsync();
                var dtos = data.Select(x => new UsuarioDto{
                    Id = x.Id,
                    Nombre = x.Nombre,
                    Apellido = x.Apellido,
                    Cedula = x.Cedula,
                    CorreoElectronico = x.CorreoElectronico,
                    Telefono = x.Telefono,
                    Verificado = x.Verificado,
                    EmpresaId = x.EmpresaId,
                    RolId = x.RolId
                });
                return Ok(new JsonResponse<IEnumerable<UsuarioDto>>(dtos));
            }
            catch (Exception ex)
            {
                return BadRequest(new JsonResponse<IEnumerable<UsuarioDto>>(null, ex.Message, ResponseStatus.error));
            }
        }

        // Panel de cuentas (admin/supervisor): todos los usuarios, incluso
        // desactivados, con nombre de rol y de empresa.
        [HttpGet("todos")]
        public async Task<ActionResult> GetTodos()
        {
            try
            {
                var data = await _usuarioService.GetTodosAsync();
                var dtos = data.Select(x => new UsuarioDto
                {
                    Id = x.Id,
                    Nombre = x.Nombre,
                    Apellido = x.Apellido,
                    Cedula = x.Cedula,
                    CorreoElectronico = x.CorreoElectronico,
                    Telefono = x.Telefono,
                    Verificado = x.Verificado,
                    EmpresaId = x.EmpresaId,
                    RolId = x.RolId,
                    NombreRol = x.Rol?.NombreRol ?? "",
                    NombreEmpresa = x.Empresa?.Nombre ?? "",
                    Activo = x.Activo,
                });
                return Ok(new JsonResponse<IEnumerable<UsuarioDto>>(dtos));
            }
            catch (Exception ex)
            {
                return BadRequest(new JsonResponse<IEnumerable<UsuarioDto>>(null, ex.Message, ResponseStatus.error));
            }
        }

        [HttpPatch("{id:int}/activar")]
        [Authorize(Roles = "Administrador,Supervisor,SuperAdmin")]
        public async Task<ActionResult> Activar(int id)
        {
            try
            {
                var userEmail = HttpContext.Items["UserEmail"]?.ToString() ?? "SYSTEM";
                var success = await _usuarioService.ReactivarAsync(id, userEmail);
                if (!success)
                    return NotFound(new JsonResponse<bool>(false, "No encontrado", ResponseStatus.error));
                return Ok(new JsonResponse<bool>(true));
            }
            catch (Exception ex)
            {
                return BadRequest(new JsonResponse<bool>(false, ex.Message, ResponseStatus.error));
            }
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult> GetById(int id)
        {
            try
            {
                var data = await _usuarioService.GetByIdAsync(id);
                if (data == null)
                    return NotFound(new JsonResponse<UsuarioDto>(null, "No encontrado", ResponseStatus.error));
                
                var dto = new UsuarioDto{
                    Id = data.Id, Nombre = data.Nombre, Apellido = data.Apellido,
                    Cedula = data.Cedula, CorreoElectronico = data.CorreoElectronico,
                    Telefono = data.Telefono, Verificado = data.Verificado,
                    EmpresaId = data.EmpresaId, RolId = data.RolId
                };
                return Ok(new JsonResponse<UsuarioDto>(dto));
            }
            catch (Exception ex)
            {
                return BadRequest(new JsonResponse<UsuarioDto>(null, ex.Message, ResponseStatus.error));
            }
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult> Login([FromBody] LoginDto credentials)
        {
            try
            {
                var user = await _context.Usuarios
                    .Include(u => u.Rol)
                    .Include(u => u.Empresa)
                    .FirstOrDefaultAsync(u => u.CorreoElectronico == credentials.CorreoElectronico && u.Activo);

                if (user == null)
                    return Unauthorized(new JsonResponse<object>((object?)null, "Credenciales incorrectas", ResponseStatus.error));

                var verification = _passwordHasher.VerifyHashedPassword(user, user.Contrasena, credentials.Contrasena);
                if (verification == PasswordVerificationResult.Failed)
                    return Unauthorized(new JsonResponse<object>((object?)null, "Credenciales incorrectas", ResponseStatus.error));

                var token = GenerateJwtToken(user);
                
                var userDto = new UsuarioDto
                {
                    Id = user.Id,
                    CorreoElectronico = user.CorreoElectronico,
                    Nombre = user.Nombre,
                    Apellido = user.Apellido,
                    Telefono = user.Telefono,
                    RolId = user.RolId,
                    NombreRol = user.Rol?.NombreRol ?? "",
                    EmpresaId = user.EmpresaId,
                    NombreEmpresa = user.Empresa?.RazonSocial ?? "", 
                    Verificado = user.Verificado
                };
                return Ok(new JsonResponse<object>(new { token, user = userDto }, "Login exitoso"));
            }
            catch (Exception ex)
            {
                return BadRequest(new JsonResponse<object>((object?)null, ex.Message, ResponseStatus.error));
            }
        }

        private string GenerateJwtToken(Usuario user)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var secretKey = Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]!);
            var expiresInHours = int.TryParse(jwtSettings["ExpiresInHours"], out var h) ? h : 8;

            var claims = new[]
            {
                new Claim("id", user.Id.ToString()),
                new Claim("email", user.CorreoElectronico),
                new Claim("rol", user.Rol?.NombreRol ?? ""),
                new Claim("empresa_id", user.EmpresaId.ToString()),
            };

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(expiresInHours),
                signingCredentials: new SigningCredentials(
                    new SymmetricSecurityKey(secretKey),
                    SecurityAlgorithms.HmacSha256));

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        [HttpPost]
        [Authorize(Roles = "Administrador,Supervisor,SuperAdmin")]
        public async Task<ActionResult> Create([FromBody] Usuario item)
        {
            try
            {
                var userEmail = HttpContext.Items["UserEmail"]?.ToString() ?? "SYSTEM";
                var result = await _usuarioService.CreateAsync(item, userEmail);

                var dto = new UsuarioDto{
                    Id = result.Id,
                    Nombre = result.Nombre,
                    Apellido = result.Apellido,
                    Cedula = result.Cedula,
                    CorreoElectronico = result.CorreoElectronico,
                    Telefono = result.Telefono,
                    Verificado = result.Verificado,
                    EmpresaId = result.EmpresaId,
                    RolId = result.RolId
                };

                return Ok(new JsonResponse<UsuarioDto>(dto));
            }
            catch (Exception ex)
            {
                var errorMsg = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return BadRequest(new JsonResponse<Usuario>(null, errorMsg, ResponseStatus.error));
            }
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "Administrador,Supervisor,SuperAdmin")]
        public async Task<ActionResult> Update(int id, [FromBody] Usuario item)
        {
            try
            {
                if (id != item.Id)
                    return BadRequest(new JsonResponse<bool>(false, "El ID no coincide", ResponseStatus.error));

                var userEmail = HttpContext.Items["UserEmail"]?.ToString() ?? "SYSTEM";
                var success = await _usuarioService.UpdateAsync(id, item, userEmail);
                
                if (!success)
                    return NotFound(new JsonResponse<bool>(false, "No encontrado", ResponseStatus.error));

                return Ok(new JsonResponse<bool>(true));
            }
            catch (Exception ex)
            {
                return BadRequest(new JsonResponse<bool>(false, ex.Message, ResponseStatus.error));
            }
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Administrador,Supervisor,SuperAdmin")]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                var userEmail = HttpContext.Items["UserEmail"]?.ToString() ?? "SYSTEM";
                var success = await _usuarioService.DeleteAsync(id, userEmail);

                if (!success)
                    return NotFound(new JsonResponse<bool>(false, "No encontrado", ResponseStatus.error));

                return Ok(new JsonResponse<bool>(true));
            }
            catch (Exception ex)
            {
                return BadRequest(new JsonResponse<bool>(false, ex.Message, ResponseStatus.error));
            }
        }

        public class UpdatePerfilDto
        {
            public string Nombre { get; set; } = null!;
            public string Apellido { get; set; } = null!;
            public string? Telefono { get; set; }
        }

        [HttpPatch("{id:int}/perfil")]
        public async Task<ActionResult> UpdatePerfil(int id, [FromBody] UpdatePerfilDto dto)
        {
            try
            {
                var userEmail = HttpContext.Items["UserEmail"]?.ToString() ?? "SYSTEM";
                var success = await _usuarioService.UpdatePerfilAsync(id, dto.Nombre, dto.Apellido, dto.Telefono, userEmail);

                if (!success)
                    return NotFound(new JsonResponse<bool>(false, "Usuario no encontrado", ResponseStatus.error));

                return Ok(new JsonResponse<bool>(true));
            }
            catch (Exception ex)
            {
                return BadRequest(new JsonResponse<bool>(false, ex.Message, ResponseStatus.error));
            }
        }

        // ── Invitación de usuario ─────────────────────────────────────────────
        // Crea la cuenta "pendiente" y envía el correo para que la persona
        // establezca su propia contraseña. RolId opcional: si no se envía, se
        // asume el rol Cliente (compatibilidad con la invitación de encargados).
        public class InvitarEncargadoDto
        {
            public string CorreoElectronico { get; set; } = null!;
            public int EmpresaId { get; set; }
            public int? RolId { get; set; }
        }

        [HttpPost("invitar")]
        [Authorize(Roles = "Administrador,Supervisor,SuperAdmin")]
        public async Task<ActionResult> Invitar([FromBody] InvitarEncargadoDto dto)
        {
            try
            {
                var userEmail = HttpContext.Items["UserEmail"]?.ToString() ?? "SYSTEM";
                await _usuarioService.InvitarUsuarioAsync(dto.CorreoElectronico, dto.RolId, dto.EmpresaId, userEmail);
                return Ok(new JsonResponse<bool>(true, "Invitación enviada"));
            }
            catch (Exception ex)
            {
                var msg = ex.InnerException?.Message ?? ex.Message;
                return BadRequest(new JsonResponse<bool>(false, msg, ResponseStatus.error));
            }
        }

        [HttpGet("invitacion/{token}")]
        [AllowAnonymous]
        public async Task<ActionResult> GetInvitacion(string token)
        {
            try
            {
                var data = await _usuarioService.GetInvitacionAsync(token);
                if (data == null)
                    return NotFound(new JsonResponse<object>((object?)null, "Invitación no válida o ya utilizada", ResponseStatus.error));

                return Ok(new JsonResponse<object>(new { correoElectronico = data.Value.correo, empresaNombre = data.Value.empresaNombre }));
            }
            catch (Exception ex)
            {
                return BadRequest(new JsonResponse<object>((object?)null, ex.Message, ResponseStatus.error));
            }
        }

        public class CompletarRegistroDto
        {
            public string Token { get; set; } = null!;
            public string Nombre { get; set; } = null!;
            public string Apellido { get; set; } = null!;
            public string Cedula { get; set; } = null!;
            public string Contrasena { get; set; } = null!;
        }

        [HttpPost("completar-registro")]
        [AllowAnonymous]
        public async Task<ActionResult> CompletarRegistro([FromBody] CompletarRegistroDto dto)
        {
            try
            {
                var ok = await _usuarioService.CompletarRegistroAsync(dto.Token, dto.Nombre, dto.Apellido, dto.Cedula, dto.Contrasena);
                if (!ok)
                    return NotFound(new JsonResponse<bool>(false, "Invitación no válida o ya utilizada", ResponseStatus.error));

                return Ok(new JsonResponse<bool>(true, "Registro completado. Ya puede iniciar sesión."));
            }
            catch (Exception ex)
            {
                var msg = ex.InnerException?.Message ?? ex.Message;
                return BadRequest(new JsonResponse<bool>(false, msg, ResponseStatus.error));
            }
        }

        public class CambiarPasswordDto
        {
            public string PasswordActual { get; set; } = null!;
            public string PasswordNueva { get; set; } = null!;
        }

        [HttpPost("{id:int}/cambiar-contrasena")]
        public async Task<ActionResult> CambiarContrasena(int id, [FromBody] CambiarPasswordDto dto)
        {
            try
            {
                var usuario = await _usuarioService.GetByIdAsync(id);
                if (usuario == null)
                    return NotFound(new JsonResponse<bool>(false, "Usuario no encontrado", ResponseStatus.error));

                var verification = _passwordHasher.VerifyHashedPassword(usuario, usuario.Contrasena, dto.PasswordActual);
                if (verification == PasswordVerificationResult.Failed)
                    return BadRequest(new JsonResponse<bool>(false, "La contraseña actual es incorrecta", ResponseStatus.error));

                var userEmail = HttpContext.Items["UserEmail"]?.ToString() ?? "SYSTEM";
                usuario.Contrasena = _passwordHasher.HashPassword(usuario, dto.PasswordNueva);
                await _usuarioService.UpdateAsync(id, usuario, userEmail);

                return Ok(new JsonResponse<bool>(true, "Contraseña actualizada correctamente"));
            }
            catch (Exception ex)
            {
                return BadRequest(new JsonResponse<bool>(false, ex.Message, ResponseStatus.error));
            }
        }

    }
}
