using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sismeing.Domain.Entities.Operaciones;
using Sismeing.Infrestructura.Persistence;

namespace Sismeing.API.Controllers.Operaciones
{
    [Authorize]
    [ApiController]
    [Route("api/foto-mantenimiento")]
    public class FotoMantenimientoController : ControllerBase
    {
        private readonly SupaBaseDBcontext _context;
        public FotoMantenimientoController(SupaBaseDBcontext context) => _context = context;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<FotoMantenimiento>>> GetAll()
            => Ok(await _context.FotosMantenimiento.Where(f => f.Activo).ToListAsync());

        [HttpGet("{id:int}")]
        public async Task<ActionResult<FotoMantenimiento>> GetById(int id)
        {
            var foto = await _context.FotosMantenimiento.FindAsync(id);
            return foto == null ? NotFound() : Ok(foto);
        }

        [HttpGet("mantenimiento/{mantenimientoId:int}")]
        public async Task<ActionResult<IEnumerable<FotoMantenimiento>>> GetByMantenimiento(int mantenimientoId)
            => Ok(await _context.FotosMantenimiento.Where(f => f.MantenimientoId == mantenimientoId && f.Activo).ToListAsync());

        [HttpPost]
        public async Task<ActionResult<FotoMantenimiento>> Create([FromBody] FotoMantenimiento foto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            foto.FechaRegistro = DateTime.UtcNow;
            foto.UsuarioRegistro = HttpContext.Items["UserEmail"]?.ToString() ?? "SYSTEM";
            _context.FotosMantenimiento.Add(foto);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = foto.Id }, foto);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var foto = await _context.FotosMantenimiento.FindAsync(id);
            if (foto == null) return NotFound();
            foto.Activo = false;
            foto.FechaEliminacion = DateTime.UtcNow;
            foto.UsuarioEliminacion = HttpContext.Items["UserEmail"]?.ToString();
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
