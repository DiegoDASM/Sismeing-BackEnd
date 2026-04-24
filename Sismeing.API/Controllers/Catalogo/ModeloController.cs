using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sismeing.Domain.Entities.Catalogo;
using Sismeing.Infrestructura.Persistence;

namespace Sismeing.API.Controllers.Catalogo
{
    [Authorize]
    [ApiController]
    [Route("api/modelo")]
    public class ModeloController : ControllerBase
    {
        private readonly SupaBaseDBcontext _context;

        public ModeloController(SupaBaseDBcontext context) => _context = context;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Modelo>>> GetAll()
            => Ok(await _context.Modelos.Where(m => m.Activo).ToListAsync());

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Modelo>> GetById(int id)
        {
            var modelo = await _context.Modelos.FindAsync(id);
            return modelo == null ? NotFound() : Ok(modelo);
        }

        [HttpPost]
        public async Task<ActionResult<Modelo>> Create([FromBody] Modelo modelo)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            modelo.FechaRegistro = DateTime.UtcNow;
            modelo.UsuarioRegistro = HttpContext.Items["UserEmail"]?.ToString() ?? "SYSTEM";
            _context.Modelos.Add(modelo);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = modelo.Id }, modelo);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] Modelo modelo)
        {
            if (id != modelo.Id) return BadRequest();
            var existente = await _context.Modelos.FindAsync(id);
            if (existente == null) return NotFound();
            existente.NombreModelo = modelo.NombreModelo;
            existente.Capacidad = modelo.Capacidad;
            existente.Potencia = modelo.Potencia;
            existente.AñoFabricacion = modelo.AñoFabricacion;
            existente.FechaModificacion = DateTime.UtcNow;
            existente.UsuarioModificacion = HttpContext.Items["UserEmail"]?.ToString();
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var modelo = await _context.Modelos.FindAsync(id);
            if (modelo == null) return NotFound();
            modelo.Activo = false;
            modelo.FechaEliminacion = DateTime.UtcNow;
            modelo.UsuarioEliminacion = HttpContext.Items["UserEmail"]?.ToString();
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
