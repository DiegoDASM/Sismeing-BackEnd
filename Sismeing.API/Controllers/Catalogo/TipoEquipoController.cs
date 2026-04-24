using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sismeing.Domain.Entities.Catalogo;
using Sismeing.Infrestructura.Persistence;

namespace Sismeing.API.Controllers.Catalogo
{
    [Authorize]
    [ApiController]
    [Route("api/tipo-equipo")]
    public class TipoEquipoController : ControllerBase
    {
        private readonly SupaBaseDBcontext _context;

        public TipoEquipoController(SupaBaseDBcontext context) => _context = context;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TipoEquipo>>> GetAll()
            => Ok(await _context.TiposEquipo.Where(t => t.Activo).ToListAsync());

        [HttpGet("{id:int}")]
        public async Task<ActionResult<TipoEquipo>> GetById(int id)
        {
            var tipo = await _context.TiposEquipo.FindAsync(id);
            return tipo == null ? NotFound() : Ok(tipo);
        }

        [HttpPost]
        public async Task<ActionResult<TipoEquipo>> Create([FromBody] TipoEquipo tipo)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            tipo.FechaRegistro = DateTime.UtcNow;
            tipo.UsuarioRegistro = HttpContext.Items["UserEmail"]?.ToString() ?? "SYSTEM";
            _context.TiposEquipo.Add(tipo);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = tipo.Id }, tipo);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] TipoEquipo tipo)
        {
            if (id != tipo.Id) return BadRequest();
            var existente = await _context.TiposEquipo.FindAsync(id);
            if (existente == null) return NotFound();
            existente.NombreTipo = tipo.NombreTipo;
            existente.FechaModificacion = DateTime.UtcNow;
            existente.UsuarioModificacion = HttpContext.Items["UserEmail"]?.ToString();
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var tipo = await _context.TiposEquipo.FindAsync(id);
            if (tipo == null) return NotFound();
            tipo.Activo = false;
            tipo.FechaEliminacion = DateTime.UtcNow;
            tipo.UsuarioEliminacion = HttpContext.Items["UserEmail"]?.ToString();
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
