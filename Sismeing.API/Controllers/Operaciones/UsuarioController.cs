using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Sismeing.Domain.Entities.Operaciones;
using Sismeing.Infrestructura.Persistence;
using Sismeing.Service;
using Sismeing.Service.Interfaces.Operaciones;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Sismeing.API.Controllers.Operaciones
{
    public class LoginDto
    {
        public string CorreoElectronico { get; set; } = null!;
        public string Contrasena { get; set; } = null!;
    }

    //[Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class UsuarioController : ControllerBase
    {
        private readonly IUsuarioService _usuarioService;
        private readonly SupaBaseDBcontext _context;
        private readonly IPasswordHasher<Usuario> _passwordHasher;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;

        public UsuarioController(
            IUsuarioService usuarioService,
            SupaBaseDBcontext context,
            IPasswordHasher<Usuario> passwordHasher,
            IConfiguration configuration,
            IWebHostEnvironment env)
        {
            _usuarioService = usuarioService;
            _context = context;
            _passwordHasher = passwordHasher;
            _configuration = configuration;
            _env = env;
        }

        [HttpGet]
        public async Task<ActionResult> GetAll()
        {
            try
            {
                var data = await _usuarioService.GetAllAsync();
                return Ok(new JsonResponse<IEnumerable<Usuario>>(data));
            }
            catch (Exception ex)
            {
                return BadRequest(new JsonResponse<IEnumerable<Usuario>>(null, ex.Message, ResponseStatus.error));
            }
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult> GetById(int id)
        {
            try
            {
                var data = await _usuarioService.GetByIdAsync(id);
                if (data == null)
                    return NotFound(new JsonResponse<Usuario>(null, "No encontrado", ResponseStatus.error));
                return Ok(new JsonResponse<Usuario>(data));
            }
            catch (Exception ex)
            {
                return BadRequest(new JsonResponse<Usuario>(null, ex.Message, ResponseStatus.error));
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
                return Ok(new JsonResponse<object>(new { token, user }, "Login exitoso"));
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
        public async Task<ActionResult> Create([FromBody] Usuario item)
        {
            try
            {
                var userEmail = HttpContext.Items["UserEmail"]?.ToString() ?? "SYSTEM";
                var result = await _usuarioService.CreateAsync(item, userEmail);

                return Ok(new JsonResponse<Usuario>(result));
            }
            catch (Exception ex)
            {
                var errorMsg = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return BadRequest(new JsonResponse<Usuario>(null, errorMsg, ResponseStatus.error));
            }
        }

        [HttpPut("{id:int}")]
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

        [HttpPost("{id:int}/upload-foto")]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult> UploadFoto(int id, IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                    return BadRequest(new JsonResponse<string>(null, "No se proporcionó archivo", ResponseStatus.error));

                var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
                var allowed = new[] { ".jpg", ".jpeg", ".png", ".webp", ".gif" };
                if (!allowed.Contains(ext))
                    return BadRequest(new JsonResponse<string>(null, "Formato no válido. Use JPG, PNG, WEBP o GIF", ResponseStatus.error));

                var webRoot = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                var uploadsPath = Path.Combine(webRoot, "uploads", "avatares");
                Directory.CreateDirectory(uploadsPath);

                var fileName = $"usuario_{id}{ext}";
                var filePath = Path.Combine(uploadsPath, fileName);

                using var stream = new FileStream(filePath, FileMode.Create);
                await file.CopyToAsync(stream);

                var fotoUrl = $"/uploads/avatares/{fileName}";
                var result = await _usuarioService.UpdateFotoAsync(id, fotoUrl);

                if (result == null)
                    return NotFound(new JsonResponse<string>(null, "Usuario no encontrado", ResponseStatus.error));

                return Ok(new JsonResponse<string>(fotoUrl));
            }
            catch (Exception ex)
            {
                return BadRequest(new JsonResponse<string>(null, ex.Message, ResponseStatus.error));
            }
        }
    }
}
