using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sismeing.Domain.Entities.Operaciones;
using Sismeing.Infrestructura.Persistence;

namespace Sismeing.API.Controllers.Operaciones
{
    [Authorize]
    [ApiController]
    [Route("api/mantenimiento")]
    public class MantenimientoController : ControllerBase
    {
        private readonly SupaBaseDBcontext _context;
        public MantenimientoController(SupaBaseDBcontext context) => _context = context;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Mantenimiento>>> GetAll()
            => Ok(await _context.Mantenimientos
                .Include(m => m.Instalacion)
                .Include(m => m.Tecnico)
                .Include(m => m.TipoMantenimiento)
                .Include(m => m.Estado)
                .Where(m => m.Activo).ToListAsync());

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Mantenimiento>> GetById(int id)
        {
            var mant = await _context.Mantenimientos
                .Include(m => m.Instalacion)
                .Include(m => m.Tecnico)
                .Include(m => m.TipoMantenimiento)
                .Include(m => m.Estado)
                .Include(m => m.Supervisor)
                .Include(m => m.Encargado)
                .Include(m => m.Fotos)
                .FirstOrDefaultAsync(m => m.Id == id);
            return mant == null ? NotFound() : Ok(mant);
        }

        [HttpGet("instalacion/{instalacionId:int}")]
        public async Task<ActionResult<IEnumerable<Mantenimiento>>> GetByInstalacion(int instalacionId)
            => Ok(await _context.Mantenimientos
                .Include(m => m.TipoMantenimiento).Include(m => m.Estado)
                .Where(m => m.InstalacionId == instalacionId && m.Activo).ToListAsync());

        [HttpPost]
        public async Task<ActionResult<Mantenimiento>> Create([FromBody] Mantenimiento mantenimiento)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            mantenimiento.FechaRegistro = DateTime.UtcNow;
            mantenimiento.UsuarioRegistro = HttpContext.Items["UserEmail"]?.ToString() ?? "SYSTEM";
            _context.Mantenimientos.Add(mantenimiento);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = mantenimiento.Id }, mantenimiento);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] Mantenimiento mantenimiento)
        {
            if (id != mantenimiento.Id) return BadRequest();
            var existente = await _context.Mantenimientos.FindAsync(id);
            if (existente == null) return NotFound();
            existente.ObservacionInicial = mantenimiento.ObservacionInicial;
            existente.ObservacionesFinales = mantenimiento.ObservacionesFinales;
            existente.RequiereRepuestos = mantenimiento.RequiereRepuestos;
            existente.TipoMantenimientoId = mantenimiento.TipoMantenimientoId;
            existente.FechaInicio = mantenimiento.FechaInicio;
            existente.FechaFin = mantenimiento.FechaFin;
            existente.FechaProximo = mantenimiento.FechaProximo;
            existente.EstadoId = mantenimiento.EstadoId;
            existente.SupervisorId = mantenimiento.SupervisorId;
            existente.EncargadoId = mantenimiento.EncargadoId;
            existente.NumeroInforme = mantenimiento.NumeroInforme;
            existente.FechaModificacion = DateTime.UtcNow;
            existente.UsuarioModificacion = HttpContext.Items["UserEmail"]?.ToString();
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var mant = await _context.Mantenimientos.FindAsync(id);
            if (mant == null) return NotFound();
            mant.Activo = false;
            mant.FechaEliminacion = DateTime.UtcNow;
            mant.UsuarioEliminacion = HttpContext.Items["UserEmail"]?.ToString();
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
