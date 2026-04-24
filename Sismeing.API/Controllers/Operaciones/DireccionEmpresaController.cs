using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sismeing.Domain.Entities.Operaciones;
using Sismeing.Infrestructura.Persistence;

namespace Sismeing.API.Controllers.Operaciones
{
    [Authorize]
    [ApiController]
    [Route("api/direccion-empresa")]
    public class DireccionEmpresaController : ControllerBase
    {
        private readonly SupaBaseDBcontext _context;
        public DireccionEmpresaController(SupaBaseDBcontext context) => _context = context;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DireccionEmpresa>>> GetAll()
            => Ok(await _context.DireccionesEmpresa.Include(d => d.Empresa).Where(d => d.Activo).ToListAsync());

        [HttpGet("{id:int}")]
        public async Task<ActionResult<DireccionEmpresa>> GetById(int id)
        {
            var dir = await _context.DireccionesEmpresa.Include(d => d.Empresa).FirstOrDefaultAsync(d => d.Id == id);
            return dir == null ? NotFound() : Ok(dir);
        }

        [HttpGet("empresa/{empresaId:int}")]
        public async Task<ActionResult<IEnumerable<DireccionEmpresa>>> GetByEmpresa(int empresaId)
            => Ok(await _context.DireccionesEmpresa.Where(d => d.EmpresaId == empresaId && d.Activo).ToListAsync());

        [HttpPost]
        public async Task<ActionResult<DireccionEmpresa>> Create([FromBody] DireccionEmpresa direccion)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            direccion.FechaRegistro = DateTime.UtcNow;
            direccion.UsuarioRegistro = HttpContext.Items["UserEmail"]?.ToString() ?? "SYSTEM";
            _context.DireccionesEmpresa.Add(direccion);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = direccion.Id }, direccion);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] DireccionEmpresa direccion)
        {
            if (id != direccion.Id) return BadRequest();
            var existente = await _context.DireccionesEmpresa.FindAsync(id);
            if (existente == null) return NotFound();
            existente.Direccion = direccion.Direccion;
            existente.EmpresaId = direccion.EmpresaId;
            existente.FechaModificacion = DateTime.UtcNow;
            existente.UsuarioModificacion = HttpContext.Items["UserEmail"]?.ToString();
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var dir = await _context.DireccionesEmpresa.FindAsync(id);
            if (dir == null) return NotFound();
            dir.Activo = false;
            dir.FechaEliminacion = DateTime.UtcNow;
            dir.UsuarioEliminacion = HttpContext.Items["UserEmail"]?.ToString();
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
