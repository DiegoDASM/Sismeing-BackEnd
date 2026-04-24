using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sismeing.Domain.Entities.Operaciones;
using Sismeing.Infrestructura.Persistence;

namespace Sismeing.API.Controllers.Operaciones
{
    [Authorize]
    [ApiController]
    [Route("api/area-empresa")]
    public class AreaEmpresaController : ControllerBase
    {
        private readonly SupaBaseDBcontext _context;
        public AreaEmpresaController(SupaBaseDBcontext context) => _context = context;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AreaEmpresa>>> GetAll()
            => Ok(await _context.AreasEmpresa.Include(a => a.Empresa).Where(a => a.Activo).ToListAsync());

        [HttpGet("{id:int}")]
        public async Task<ActionResult<AreaEmpresa>> GetById(int id)
        {
            var area = await _context.AreasEmpresa.Include(a => a.Empresa).FirstOrDefaultAsync(a => a.Id == id);
            return area == null ? NotFound() : Ok(area);
        }

        [HttpGet("empresa/{empresaId:int}")]
        public async Task<ActionResult<IEnumerable<AreaEmpresa>>> GetByEmpresa(int empresaId)
            => Ok(await _context.AreasEmpresa.Where(a => a.EmpresaId == empresaId && a.Activo).ToListAsync());

        [HttpPost]
        public async Task<ActionResult<AreaEmpresa>> Create([FromBody] AreaEmpresa area)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            area.FechaRegistro = DateTime.UtcNow;
            area.UsuarioRegistro = HttpContext.Items["UserEmail"]?.ToString() ?? "SYSTEM";
            _context.AreasEmpresa.Add(area);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = area.Id }, area);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] AreaEmpresa area)
        {
            if (id != area.Id) return BadRequest();
            var existente = await _context.AreasEmpresa.FindAsync(id);
            if (existente == null) return NotFound();
            existente.NombreArea = area.NombreArea;
            existente.EmpresaId = area.EmpresaId;
            existente.FechaModificacion = DateTime.UtcNow;
            existente.UsuarioModificacion = HttpContext.Items["UserEmail"]?.ToString();
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var area = await _context.AreasEmpresa.FindAsync(id);
            if (area == null) return NotFound();
            area.Activo = false;
            area.FechaEliminacion = DateTime.UtcNow;
            area.UsuarioEliminacion = HttpContext.Items["UserEmail"]?.ToString();
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
