using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sismeing.Domain.Entities.Operaciones;
using Sismeing.Infrestructura.Persistence;

namespace Sismeing.API.Controllers.Operaciones
{
    [Authorize]
    [ApiController]
    [Route("api/contrato")]
    public class ContratoController : ControllerBase
    {
        private readonly SupaBaseDBcontext _context;
        public ContratoController(SupaBaseDBcontext context) => _context = context;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Contrato>>> GetAll()
            => Ok(await _context.Contratos
                .Include(c => c.Empresa)
                .Include(c => c.Direccion)
                .Include(c => c.Encargado)
                .Include(c => c.TipoTrabajo)
                .Where(c => c.Activo).ToListAsync());

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Contrato>> GetById(int id)
        {
            var contrato = await _context.Contratos
                .Include(c => c.Empresa)
                .Include(c => c.Direccion)
                .Include(c => c.Encargado)
                .Include(c => c.TipoTrabajo)
                .Include(c => c.Equipos)
                .FirstOrDefaultAsync(c => c.Id == id);
            return contrato == null ? NotFound() : Ok(contrato);
        }

        [HttpPost]
        public async Task<ActionResult<Contrato>> Create([FromBody] Contrato contrato)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            contrato.FechaRegistro = DateTime.UtcNow;
            contrato.UsuarioRegistro = HttpContext.Items["UserEmail"]?.ToString() ?? "SYSTEM";
            _context.Contratos.Add(contrato);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = contrato.Id }, contrato);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] Contrato contrato)
        {
            if (id != contrato.Id) return BadRequest();
            var existente = await _context.Contratos.FindAsync(id);
            if (existente == null) return NotFound();
            existente.NombreProyecto = contrato.NombreProyecto;
            existente.EmpresaId = contrato.EmpresaId;
            existente.DireccionId = contrato.DireccionId;
            existente.EncargadoId = contrato.EncargadoId;
            existente.TipoTrabajoId = contrato.TipoTrabajoId;
            existente.FechaInicio = contrato.FechaInicio;
            existente.FechaFin = contrato.FechaFin;
            existente.FechaModificacion = DateTime.UtcNow;
            existente.UsuarioModificacion = HttpContext.Items["UserEmail"]?.ToString();
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var contrato = await _context.Contratos.FindAsync(id);
            if (contrato == null) return NotFound();
            contrato.Activo = false;
            contrato.FechaEliminacion = DateTime.UtcNow;
            contrato.UsuarioEliminacion = HttpContext.Items["UserEmail"]?.ToString();
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
