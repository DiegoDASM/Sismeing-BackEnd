using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sismeing.Domain.Entities.Catalogo;
using Sismeing.Infrestructura.Persistence;

namespace Sismeing.API.Controllers.Catalogo
{
    [Authorize]
    [ApiController]
    [Route("api/estado")]
    public class EstadoController : ControllerBase
    {
        private readonly SupaBaseDBcontext _context;

        public EstadoController(SupaBaseDBcontext context) => _context = context;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Estado>>> GetAll()
            => Ok(await _context.Estados.Where(e => e.Activo).ToListAsync());

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Estado>> GetById(int id)
        {
            var estado = await _context.Estados.FindAsync(id);
            return estado == null ? NotFound() : Ok(estado);
        }

        [HttpPost]
        public async Task<ActionResult<Estado>> Create([FromBody] Estado estado)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            estado.FechaRegistro = DateTime.UtcNow;
            estado.UsuarioRegistro = HttpContext.Items["UserEmail"]?.ToString() ?? "SYSTEM";
            _context.Estados.Add(estado);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = estado.Id }, estado);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] Estado estado)
        {
            if (id != estado.Id) return BadRequest();
            var existente = await _context.Estados.FindAsync(id);
            if (existente == null) return NotFound();
            existente.NombreEstado = estado.NombreEstado;
            existente.FechaModificacion = DateTime.UtcNow;
            existente.UsuarioModificacion = HttpContext.Items["UserEmail"]?.ToString();
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var estado = await _context.Estados.FindAsync(id);
            if (estado == null) return NotFound();
            estado.Activo = false;
            estado.FechaEliminacion = DateTime.UtcNow;
            estado.UsuarioEliminacion = HttpContext.Items["UserEmail"]?.ToString();
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
