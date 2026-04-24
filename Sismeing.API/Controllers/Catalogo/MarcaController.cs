using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sismeing.Domain.Entities.Catalogo;
using Sismeing.Infrestructura.Persistence;

namespace Sismeing.API.Controllers.Catalogo
{
    [Authorize]
    [ApiController]
    [Route("api/marca")]
    public class MarcaController : ControllerBase
    {
        private readonly SupaBaseDBcontext _context;

        public MarcaController(SupaBaseDBcontext context) => _context = context;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Marca>>> GetAll()
            => Ok(await _context.Marcas.Where(m => m.Activo).ToListAsync());

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Marca>> GetById(int id)
        {
            var marca = await _context.Marcas.FindAsync(id);
            return marca == null ? NotFound() : Ok(marca);
        }

        [HttpPost]
        public async Task<ActionResult<Marca>> Create([FromBody] Marca marca)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            marca.FechaRegistro = DateTime.UtcNow;
            marca.UsuarioRegistro = HttpContext.Items["UserEmail"]?.ToString() ?? "SYSTEM";
            _context.Marcas.Add(marca);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = marca.Id }, marca);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] Marca marca)
        {
            if (id != marca.Id) return BadRequest();
            var existente = await _context.Marcas.FindAsync(id);
            if (existente == null) return NotFound();
            existente.NombreMarca = marca.NombreMarca;
            existente.FechaModificacion = DateTime.UtcNow;
            existente.UsuarioModificacion = HttpContext.Items["UserEmail"]?.ToString();
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var marca = await _context.Marcas.FindAsync(id);
            if (marca == null) return NotFound();
            marca.Activo = false;
            marca.FechaEliminacion = DateTime.UtcNow;
            marca.UsuarioEliminacion = HttpContext.Items["UserEmail"]?.ToString();
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
