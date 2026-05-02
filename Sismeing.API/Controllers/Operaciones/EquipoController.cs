using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sismeing.Domain.Entities.Operaciones;
using Sismeing.Infrestructura.Persistence;
using Sismeing.Service;

namespace Sismeing.API.Controllers.Operaciones
{
    //[Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class EquipoController : Controller
    {
        private readonly SupaBaseDBcontext _context;

        public EquipoController(SupaBaseDBcontext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult> GetAll()
        {
            try
            {
                var data = await _context.Equipos.ToListAsync();
                return Ok(new JsonResponse<IEnumerable<Equipo>>(data));
            }
            catch (Exception ex)
            {
                return BadRequest(new JsonResponse<IEnumerable<Equipo>>(null, ex.Message, ResponseStatus.error));
            }
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult> GetById(int id)
        {
            try
            {
                var data = await _context.Equipos.FindAsync(id);
                if (data == null)
                    return NotFound(new JsonResponse<Equipo>(null, "No encontrado", ResponseStatus.error));
                return Ok(new JsonResponse<Equipo>(data));
            }
            catch (Exception ex)
            {
                return BadRequest(new JsonResponse<Equipo>(null, ex.Message, ResponseStatus.error));
            }
        }

        [HttpPost]
        public async Task<ActionResult> Create([FromBody] Equipo item)
        {
            try
            {
                item.UsuarioRegistro = HttpContext.Items["UserEmail"]?.ToString() ?? "SYSTEM";
                item.FechaRegistro = DateTime.UtcNow;
                item.Activo = true;

                _context.Equipos.Add(item);
                await _context.SaveChangesAsync();

                return Ok(new JsonResponse<Equipo>(item));
            }
            catch (Exception ex)
            {
                return BadRequest(new JsonResponse<Equipo>(null, ex.Message, ResponseStatus.error));
            }
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Update(int id, [FromBody] Equipo item)
        {
            try
            {
                if (id != item.Id)
                    return BadRequest(new JsonResponse<bool>(false, "El ID no coincide", ResponseStatus.error));

                var existingItem = await _context.Equipos.FindAsync(id);
                if (existingItem == null)
                    return NotFound(new JsonResponse<bool>(false, "No encontrado", ResponseStatus.error));

                _context.Entry(existingItem).CurrentValues.SetValues(item);
                existingItem.UsuarioModificacion = HttpContext.Items["UserEmail"]?.ToString() ?? "SYSTEM";
                existingItem.FechaModificacion = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                return Ok(new JsonResponse<bool>(true));
            }
            catch (Exception ex)
            {
                return BadRequest(new JsonResponse<bool>(false, ex.Message, ResponseStatus.error));
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                var existingItem = await _context.Equipos.FindAsync(id);
                if (existingItem == null)
                    return NotFound(new JsonResponse<bool>(false, "No encontrado", ResponseStatus.error));

                existingItem.Activo = false;
                existingItem.UsuarioEliminacion = HttpContext.Items["UserEmail"]?.ToString() ?? "SYSTEM";
                existingItem.FechaEliminacion = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                return Ok(new JsonResponse<bool>(true));
            }
            catch (Exception ex)
            {
                return BadRequest(new JsonResponse<bool>(false, ex.Message, ResponseStatus.error));
            }
        }
    }
}
