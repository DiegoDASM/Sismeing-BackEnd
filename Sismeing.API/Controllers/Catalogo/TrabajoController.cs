using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sismeing.Domain.Entities.Catalogo;
using Sismeing.Infrestructura.Persistence;

namespace Sismeing.API.Controllers.Catalogo
{
    [Authorize]
    [ApiController]
    [Route("api/trabajo")]
    public class TrabajoController : ControllerBase
    {
        private readonly SupaBaseDBcontext _context;
        public TrabajoController(SupaBaseDBcontext context) => _context = context;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Trabajo>>> GetAll()
            => Ok(await _context.Trabajos.Where(t => t.Activo).ToListAsync());

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Trabajo>> GetById(int id)
        {
            var t = await _context.Trabajos.FindAsync(id);
            return t == null ? NotFound() : Ok(t);
        }

        [HttpPost]
        public async Task<ActionResult<Trabajo>> Create([FromBody] Trabajo trabajo)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            trabajo.FechaRegistro = DateTime.UtcNow;
            trabajo.UsuarioRegistro = HttpContext.Items["UserEmail"]?.ToString() ?? "SYSTEM";
            _context.Trabajos.Add(trabajo);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = trabajo.Id }, trabajo);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] Trabajo trabajo)
        {
            if (id != trabajo.Id) return BadRequest();
            var existente = await _context.Trabajos.FindAsync(id);
            if (existente == null) return NotFound();
            existente.NombreTrabajo = trabajo.NombreTrabajo;
            existente.Descripcion = trabajo.Descripcion;
            existente.FechaModificacion = DateTime.UtcNow;
            existente.UsuarioModificacion = HttpContext.Items["UserEmail"]?.ToString();
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var trabajo = await _context.Trabajos.FindAsync(id);
            if (trabajo == null) return NotFound();
            trabajo.Activo = false;
            trabajo.FechaEliminacion = DateTime.UtcNow;
            trabajo.UsuarioEliminacion = HttpContext.Items["UserEmail"]?.ToString();
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
