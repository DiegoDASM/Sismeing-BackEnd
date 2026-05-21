using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sismeing.Domain.Entities.Operaciones;
using Sismeing.Infrestructura.Persistence;
using Sismeing.Service;

using Sismeing.Service;

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
    public class UsuarioController : Controller
    {
        private readonly SupaBaseDBcontext _context;

        public UsuarioController(SupaBaseDBcontext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult> GetAll()
        {
            try
            {
                var data = await _context.Usuarios.ToListAsync();
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
                var data = await _context.Usuarios.FindAsync(id);
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
                    .FirstOrDefaultAsync(u => u.CorreoElectronico == credentials.CorreoElectronico && u.Contrasena == credentials.Contrasena && u.Activo);

                if (user == null)
                    return Unauthorized(new JsonResponse<Usuario>(null, "Credenciales incorrectas", ResponseStatus.error));

                // NOTA: Para un entorno real, aquí se generaría el JWT token usando la clave secreta en appsettings.json.
                // Como solución temporal para que el frontend funcione, devolvemos un token ficticio y los datos del usuario.
                return Ok(new
                {
                    status = "success",
                    message = "Login exitoso",
                    token = "fake-jwt-token-replace-with-real-token",
                    data = user
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new JsonResponse<Usuario>(null, ex.Message, ResponseStatus.error));
            }
        }

        [HttpPost]
        public async Task<ActionResult> Create([FromBody] Usuario item)
        {
            try
            {
                item.UsuarioRegistro = HttpContext.Items["UserEmail"]?.ToString() ?? "SYSTEM";
                item.FechaRegistro = DateTime.UtcNow;
                item.Activo = true;

                _context.Usuarios.Add(item);
                await _context.SaveChangesAsync();

                return Ok(new JsonResponse<Usuario>(item));
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

                var existingItem = await _context.Usuarios.FindAsync(id);
                if (existingItem == null)
                    return NotFound(new JsonResponse<bool>(false, "No encontrado", ResponseStatus.error));

                _context.Entry(existingItem).CurrentValues.SetValues(item);
                existingItem.UsuarioModificacion = HttpContext.Items["UserEmail"]?.ToString() ?? "SYSTEM";
                existingItem.FechaModificacion = DateTime.UtcNow;
                await _context.SaveChangesAsync();

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
                var existingItem = await _context.Usuarios.FindAsync(id);
                if (existingItem == null)
                    return NotFound(new JsonResponse<bool>(false, "No encontrado", ResponseStatus.error));

                existingItem.Activo = false;
                existingItem.UsuarioEliminacion = HttpContext.Items["UserEmail"]?.ToString() ?? "SYSTEM";
                existingItem.FechaEliminacion = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                return Ok(new JsonResponse<bool>(true));
            }
            catch (Exception ex)
            {
                return BadRequest(new JsonResponse<bool>(false, ex.Message, ResponseStatus.error));
            }
        }
    }
}
