using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sismeing.Domain.Entities.Operaciones;
using Sismeing.Infrestructura.Persistence;

namespace Sismeing.API.Controllers.Operaciones
{
    [Authorize]
    [ApiController]
    [Route("api/foto-instalacion")]
    public class FotoInstalacionController : ControllerBase
    {
        private readonly SupaBaseDBcontext _context;
        public FotoInstalacionController(SupaBaseDBcontext context) => _context = context;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<FotoInstalacion>>> GetAll()
            => Ok(await _context.FotosInstalacion.Where(f => f.Activo).ToListAsync());

        [HttpGet("{id:int}")]
        public async Task<ActionResult<FotoInstalacion>> GetById(int id)
        {
            var foto = await _context.FotosInstalacion.FindAsync(id);
            return foto == null ? NotFound() : Ok(foto);
        }

        [HttpGet("instalacion/{instalacionId:int}")]
        public async Task<ActionResult<IEnumerable<FotoInstalacion>>> GetByInstalacion(int instalacionId)
            => Ok(await _context.FotosInstalacion.Where(f => f.InstalacionId == instalacionId && f.Activo).ToListAsync());

        [HttpPost]
        public async Task<ActionResult<FotoInstalacion>> Create([FromBody] FotoInstalacion foto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            foto.FechaRegistro = DateTime.UtcNow;
            foto.UsuarioRegistro = HttpContext.Items["UserEmail"]?.ToString() ?? "SYSTEM";
            _context.FotosInstalacion.Add(foto);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = foto.Id }, foto);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var foto = await _context.FotosInstalacion.FindAsync(id);
            if (foto == null) return NotFound();
            foto.Activo = false;
            foto.FechaEliminacion = DateTime.UtcNow;
            foto.UsuarioEliminacion = HttpContext.Items["UserEmail"]?.ToString();
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
