using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sismeing.Domain.Entities.Operaciones;
using Sismeing.Infrestructura.Persistence;

namespace Sismeing.API.Controllers.Operaciones
{
    [Authorize]
    [ApiController]
    [Route("api/empresa")]
    public class EmpresaController : ControllerBase
    {
        private readonly SupaBaseDBcontext _context;
        public EmpresaController(SupaBaseDBcontext context) => _context = context;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Empresa>>> GetAll()
            => Ok(await _context.Empresas.Where(e => e.Activo).ToListAsync());

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Empresa>> GetById(int id)
        {
            var empresa = await _context.Empresas
                .Include(e => e.AreasEmpresa)
                .Include(e => e.DireccionesEmpresa)
                .FirstOrDefaultAsync(e => e.Id == id);
            return empresa == null ? NotFound() : Ok(empresa);
        }

        [HttpPost]
        public async Task<ActionResult<Empresa>> Create([FromBody] Empresa empresa)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            empresa.FechaRegistro = DateTime.UtcNow;
            empresa.UsuarioRegistro = HttpContext.Items["UserEmail"]?.ToString() ?? "SYSTEM";
            _context.Empresas.Add(empresa);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = empresa.Id }, empresa);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] Empresa empresa)
        {
            if (id != empresa.Id) return BadRequest();
            var existente = await _context.Empresas.FindAsync(id);
            if (existente == null) return NotFound();
            existente.Nombre = empresa.Nombre;
            existente.RazonSocial = empresa.RazonSocial;
            existente.Telefono = empresa.Telefono;
            existente.CorreoElectronico = empresa.CorreoElectronico;
            existente.Logo = empresa.Logo;
            existente.FechaModificacion = DateTime.UtcNow;
            existente.UsuarioModificacion = HttpContext.Items["UserEmail"]?.ToString();
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var empresa = await _context.Empresas.FindAsync(id);
            if (empresa == null) return NotFound();
            empresa.Activo = false;
            empresa.FechaEliminacion = DateTime.UtcNow;
            empresa.UsuarioEliminacion = HttpContext.Items["UserEmail"]?.ToString();
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
