using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sismeing.Domain.Entities.Catalogo;
using Sismeing.Infrestructura.Persistence;

namespace Sismeing.API.Controllers.Catalogo
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class MarcaController : ControllerBase
    {
        private readonly SupaBaseDBcontext _context;

        public MarcaController(SupaBaseDBcontext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Marca>>> GetAll()
        {
            return Ok(await _context.Marcas.Where(x => x.Activo).ToListAsync());
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Marca>> GetById(int id)
        {
            var item = await _context.Marcas.FirstOrDefaultAsync(x => x.Id == id);
            return item == null ? NotFound() : Ok(item);
        }

        [HttpPost]
        public async Task<ActionResult<Marca>> Create([FromBody] Marca item)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            item.FechaRegistro = DateTime.UtcNow;
            item.UsuarioRegistro = HttpContext.Items["UserEmail"]?.ToString() ?? "SYSTEM";

            _context.Marcas.Add(item);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = item.Id }, item);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] Marca item)
        {
            if (id != item.Id) return BadRequest();

            var existente = await _context.Marcas.FindAsync(id);
            if (existente == null) return NotFound();

            var fechaRegistro = existente.FechaRegistro;
            var usuarioRegistro = existente.UsuarioRegistro;

            _context.Entry(existente).CurrentValues.SetValues(item);

            existente.FechaRegistro = fechaRegistro;
            existente.UsuarioRegistro = usuarioRegistro;
            existente.FechaModificacion = DateTime.UtcNow;
            existente.UsuarioModificacion = HttpContext.Items["UserEmail"]?.ToString() ?? "SYSTEM";

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _context.Marcas.FindAsync(id);
            if (item == null) return NotFound();

            item.Activo = false;
            item.FechaEliminacion = DateTime.UtcNow;
            item.UsuarioEliminacion = HttpContext.Items["UserEmail"]?.ToString() ?? "SYSTEM";

            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
