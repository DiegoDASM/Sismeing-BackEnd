using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sismeing.Domain.Entities.Operaciones;
using Sismeing.Infrestructura.Persistence;

namespace Sismeing.API.Controllers.Operaciones
{
    [Authorize]
    [ApiController]
    [Route("api/equipo")]
    public class EquipoController : ControllerBase
    {
        private readonly SupaBaseDBcontext _context;
        public EquipoController(SupaBaseDBcontext context) => _context = context;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Equipo>>> GetAll()
            => Ok(await _context.Equipos
                .Include(e => e.Marca)
                .Include(e => e.Tipo)
                .Include(e => e.Modelo)
                .Where(e => e.Activo).ToListAsync());

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Equipo>> GetById(int id)
        {
            var equipo = await _context.Equipos
                .Include(e => e.Marca)
                .Include(e => e.Tipo)
                .Include(e => e.Modelo)
                .Include(e => e.Proyecto)
                .FirstOrDefaultAsync(e => e.Id == id);
            return equipo == null ? NotFound() : Ok(equipo);
        }

        [HttpGet("proyecto/{proyectoId:int}")]
        public async Task<ActionResult<IEnumerable<Equipo>>> GetByProyecto(int proyectoId)
            => Ok(await _context.Equipos
                .Include(e => e.Marca).Include(e => e.Tipo).Include(e => e.Modelo)
                .Where(e => e.ProyectoId == proyectoId && e.Activo).ToListAsync());

        [HttpPost]
        public async Task<ActionResult<Equipo>> Create([FromBody] Equipo equipo)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            equipo.FechaRegistro = DateTime.UtcNow;
            equipo.UsuarioRegistro = HttpContext.Items["UserEmail"]?.ToString() ?? "SYSTEM";
            _context.Equipos.Add(equipo);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = equipo.Id }, equipo);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] Equipo equipo)
        {
            if (id != equipo.Id) return BadRequest();
            var existente = await _context.Equipos.FindAsync(id);
            if (existente == null) return NotFound();
            existente.Nombre = equipo.Nombre;
            existente.MarcaId = equipo.MarcaId;
            existente.TipoId = equipo.TipoId;
            existente.ModeloId = equipo.ModeloId;
            existente.Codigo = equipo.Codigo;
            existente.NumeroSerie = equipo.NumeroSerie;
            existente.ProyectoId = equipo.ProyectoId;
            existente.FechaModificacion = DateTime.UtcNow;
            existente.UsuarioModificacion = HttpContext.Items["UserEmail"]?.ToString();
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var equipo = await _context.Equipos.FindAsync(id);
            if (equipo == null) return NotFound();
            equipo.Activo = false;
            equipo.FechaEliminacion = DateTime.UtcNow;
            equipo.UsuarioEliminacion = HttpContext.Items["UserEmail"]?.ToString();
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
