using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sismeing.Domain.Entities.Catalogo;
using Sismeing.Infrestructura.Persistence;

namespace Sismeing.API.Controllers.Catalogo
{
    [Authorize]
    [ApiController]
    [Route("api/rol")]
    public class RolController : ControllerBase
    {
        private readonly SupaBaseDBcontext _context;

        public RolController(SupaBaseDBcontext context) => _context = context;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Rol>>> GetAll()
            => Ok(await _context.Roles.Where(r => r.Activo).ToListAsync());

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Rol>> GetById(int id)
        {
            var rol = await _context.Roles.FindAsync(id);
            return rol == null ? NotFound() : Ok(rol);
        }

        [HttpPost]
        public async Task<ActionResult<Rol>> Create([FromBody] Rol rol)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            rol.FechaRegistro = DateTime.UtcNow;
            rol.UsuarioRegistro = HttpContext.Items["UserEmail"]?.ToString() ?? "SYSTEM";
            _context.Roles.Add(rol);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = rol.Id }, rol);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] Rol rol)
        {
            if (id != rol.Id) return BadRequest();
            var existente = await _context.Roles.FindAsync(id);
            if (existente == null) return NotFound();
            existente.NombreRol = rol.NombreRol;
            existente.FechaModificacion = DateTime.UtcNow;
            existente.UsuarioModificacion = HttpContext.Items["UserEmail"]?.ToString();
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var rol = await _context.Roles.FindAsync(id);
            if (rol == null) return NotFound();
            rol.Activo = false;
            rol.FechaEliminacion = DateTime.UtcNow;
            rol.UsuarioEliminacion = HttpContext.Items["UserEmail"]?.ToString();
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
