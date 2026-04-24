using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sismeing.Domain.Entities.Operaciones;
using Sismeing.Infrestructura.Persistence;

namespace Sismeing.API.Controllers.Operaciones
{
    [Authorize]
    [ApiController]
    [Route("api/instalacion")]
    public class InstalacionController : ControllerBase
    {
        private readonly SupaBaseDBcontext _context;
        public InstalacionController(SupaBaseDBcontext context) => _context = context;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Instalacion>>> GetAll()
            => Ok(await _context.Instalaciones
                .Include(i => i.Equipo)
                .Include(i => i.Area)
                .Include(i => i.Tecnico)
                .Include(i => i.Estado)
                .Where(i => i.Activo).ToListAsync());

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Instalacion>> GetById(int id)
        {
            var instalacion = await _context.Instalaciones
                .Include(i => i.Equipo).Include(i => i.Area)
                .Include(i => i.Tecnico).Include(i => i.Estado)
                .Include(i => i.Fotos)
                .FirstOrDefaultAsync(i => i.Id == id);
            return instalacion == null ? NotFound() : Ok(instalacion);
        }

        [HttpPost]
        public async Task<ActionResult<Instalacion>> Create([FromBody] Instalacion instalacion)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            instalacion.FechaRegistro = DateTime.UtcNow;
            instalacion.UsuarioRegistro = HttpContext.Items["UserEmail"]?.ToString() ?? "SYSTEM";
            _context.Instalaciones.Add(instalacion);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = instalacion.Id }, instalacion);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] Instalacion instalacion)
        {
            if (id != instalacion.Id) return BadRequest();
            var existente = await _context.Instalaciones.FindAsync(id);
            if (existente == null) return NotFound();
            existente.EquipoId = instalacion.EquipoId;
            existente.AreaId = instalacion.AreaId;
            existente.TecnicoId = instalacion.TecnicoId;
            existente.OrdenTrabajo = instalacion.OrdenTrabajo;
            existente.HorasTrabajadas = instalacion.HorasTrabajadas;
            existente.FechaInicio = instalacion.FechaInicio;
            existente.FechaFin = instalacion.FechaFin;
            existente.EstadoId = instalacion.EstadoId;
            existente.NumeroInforme = instalacion.NumeroInforme;
            existente.FechaModificacion = DateTime.UtcNow;
            existente.UsuarioModificacion = HttpContext.Items["UserEmail"]?.ToString();
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var instalacion = await _context.Instalaciones.FindAsync(id);
            if (instalacion == null) return NotFound();
            instalacion.Activo = false;
            instalacion.FechaEliminacion = DateTime.UtcNow;
            instalacion.UsuarioEliminacion = HttpContext.Items["UserEmail"]?.ToString();
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
