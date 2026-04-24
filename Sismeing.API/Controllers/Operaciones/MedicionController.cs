using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sismeing.Domain.Entities.Operaciones;
using Sismeing.Infrestructura.Persistence;

namespace Sismeing.API.Controllers.Operaciones
{
    [Authorize]
    [ApiController]
    [Route("api/medicion")]
    public class MedicionController : ControllerBase
    {
        private readonly SupaBaseDBcontext _context;
        public MedicionController(SupaBaseDBcontext context) => _context = context;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Medicion>>> GetAll()
            => Ok(await _context.Mediciones
                .Include(m => m.Equipo).Include(m => m.Area)
                .Where(m => m.Activo).ToListAsync());

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Medicion>> GetById(int id)
        {
            var medicion = await _context.Mediciones
                .Include(m => m.Equipo).Include(m => m.Area)
                .FirstOrDefaultAsync(m => m.Id == id);
            return medicion == null ? NotFound() : Ok(medicion);
        }

        [HttpGet("equipo/{equipoId:int}")]
        public async Task<ActionResult<IEnumerable<Medicion>>> GetByEquipo(int equipoId)
            => Ok(await _context.Mediciones.Where(m => m.EquipoId == equipoId && m.Activo).ToListAsync());

        [HttpPost]
        public async Task<ActionResult<Medicion>> Create([FromBody] Medicion medicion)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            medicion.FechaRegistro = DateTime.UtcNow;
            medicion.UsuarioRegistro = HttpContext.Items["UserEmail"]?.ToString() ?? "SYSTEM";
            _context.Mediciones.Add(medicion);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = medicion.Id }, medicion);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] Medicion medicion)
        {
            if (id != medicion.Id) return BadRequest();
            var existente = await _context.Mediciones.FindAsync(id);
            if (existente == null) return NotFound();
            existente.Voltaje = medicion.Voltaje;
            existente.Frecuencia = medicion.Frecuencia;
            existente.AmpEvaporadorVentiladorRla = medicion.AmpEvaporadorVentiladorRla;
            existente.AmpMotorCondensadoraRla = medicion.AmpMotorCondensadoraRla;
            existente.AmpCompresorRla = medicion.AmpCompresorRla;
            existente.PresionSuccionPsi = medicion.PresionSuccionPsi;
            existente.PresionDescargaPsi = medicion.PresionDescargaPsi;
            existente.TempInicialFinalEvapC = medicion.TempInicialFinalEvapC;
            existente.TempInicialFinalCondC = medicion.TempInicialFinalCondC;
            existente.TempIngresoSalidaAguaC = medicion.TempIngresoSalidaAguaC;
            existente.TemperaturaProgramadaC = medicion.TemperaturaProgramadaC;
            existente.HumedadRelativaProgPct = medicion.HumedadRelativaProgPct;
            existente.Inicial = medicion.Inicial;
            existente.FechaModificacion = DateTime.UtcNow;
            existente.UsuarioModificacion = HttpContext.Items["UserEmail"]?.ToString();
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var medicion = await _context.Mediciones.FindAsync(id);
            if (medicion == null) return NotFound();
            medicion.Activo = false;
            medicion.FechaEliminacion = DateTime.UtcNow;
            medicion.UsuarioEliminacion = HttpContext.Items["UserEmail"]?.ToString();
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
