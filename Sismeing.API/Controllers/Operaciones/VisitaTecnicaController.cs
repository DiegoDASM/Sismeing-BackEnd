using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sismeing.Domain.Entities.Operaciones;
using Sismeing.Infrestructura.Persistence;

namespace Sismeing.API.Controllers.Operaciones
{
    [Authorize]
    [ApiController]
    [Route("api/visita-tecnica")]
    public class VisitaTecnicaController : ControllerBase
    {
        private readonly SupaBaseDBcontext _context;
        public VisitaTecnicaController(SupaBaseDBcontext context) => _context = context;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<VisitaTecnica>>> GetAll()
            => Ok(await _context.VisitasTecnicas
                .Include(v => v.Empresa)
                .Include(v => v.Tecnico)
                .Include(v => v.TipoTrabajo)
                .Where(v => v.Activo).ToListAsync());

        [HttpGet("{id:int}")]
        public async Task<ActionResult<VisitaTecnica>> GetById(int id)
        {
            var visita = await _context.VisitasTecnicas
                .Include(v => v.Empresa).Include(v => v.Tecnico).Include(v => v.TipoTrabajo)
                .FirstOrDefaultAsync(v => v.Id == id);
            return visita == null ? NotFound() : Ok(visita);
        }

        [HttpGet("empresa/{empresaId:int}")]
        public async Task<ActionResult<IEnumerable<VisitaTecnica>>> GetByEmpresa(int empresaId)
            => Ok(await _context.VisitasTecnicas
                .Include(v => v.Tecnico).Include(v => v.TipoTrabajo)
                .Where(v => v.EmpresaId == empresaId && v.Activo).ToListAsync());

        [HttpPost]
        public async Task<ActionResult<VisitaTecnica>> Create([FromBody] VisitaTecnica visita)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            visita.FechaRegistro = DateTime.UtcNow;
            visita.UsuarioRegistro = HttpContext.Items["UserEmail"]?.ToString() ?? "SYSTEM";
            _context.VisitasTecnicas.Add(visita);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = visita.Id }, visita);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] VisitaTecnica visita)
        {
            if (id != visita.Id) return BadRequest();
            var existente = await _context.VisitasTecnicas.FindAsync(id);
            if (existente == null) return NotFound();
            existente.EmpresaId = visita.EmpresaId;
            existente.TecnicoId = visita.TecnicoId;
            existente.TipoTrabajoId = visita.TipoTrabajoId;
            existente.FechaVisita = visita.FechaVisita;
            existente.DescripcionVisita = visita.DescripcionVisita;
            existente.Observaciones = visita.Observaciones;
            existente.NumeroInforme = visita.NumeroInforme;
            existente.FechaModificacion = DateTime.UtcNow;
            existente.UsuarioModificacion = HttpContext.Items["UserEmail"]?.ToString();
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var visita = await _context.VisitasTecnicas.FindAsync(id);
            if (visita == null) return NotFound();
            visita.Activo = false;
            visita.FechaEliminacion = DateTime.UtcNow;
            visita.UsuarioEliminacion = HttpContext.Items["UserEmail"]?.ToString();
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
