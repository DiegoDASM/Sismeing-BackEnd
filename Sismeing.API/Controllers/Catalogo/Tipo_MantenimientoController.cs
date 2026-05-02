using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sismeing.Domain.Entities.Catalogo;
using Sismeing.Infrestructura.Persistence;
using Sismeing.Service;

namespace Sismeing.API.Controllers.Catalogo
{
    //[Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class Tipo_MantenimientoController : Controller
    {
        private readonly SupaBaseDBcontext _context;

        public Tipo_MantenimientoController(SupaBaseDBcontext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult> GetAll()
        {
            try
            {
                var data = await _context.TiposMantenimiento.ToListAsync();
                return Ok(new JsonResponse<IEnumerable<Tipo_Mantenimiento>>(data));
            }
            catch (Exception ex)
            {
                return BadRequest(new JsonResponse<IEnumerable<Tipo_Mantenimiento>>(null, ex.Message, ResponseStatus.error));
            }
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult> GetById(int id)
        {
            try
            {
                var data = await _context.TiposMantenimiento.FindAsync(id);
                if (data == null)
                    return NotFound(new JsonResponse<Tipo_Mantenimiento>(null, "No encontrado", ResponseStatus.error));
                return Ok(new JsonResponse<Tipo_Mantenimiento>(data));
            }
            catch (Exception ex)
            {
                return BadRequest(new JsonResponse<Tipo_Mantenimiento>(null, ex.Message, ResponseStatus.error));
            }
        }

        [HttpPost]
        public async Task<ActionResult> Create([FromBody] Tipo_Mantenimiento item)
        {
            try
            {
                item.UsuarioRegistro = HttpContext.Items["UserEmail"]?.ToString() ?? "SYSTEM";
                item.FechaRegistro = DateTime.UtcNow;
                item.Activo = true;

                _context.TiposMantenimiento.Add(item);
                await _context.SaveChangesAsync();

                return Ok(new JsonResponse<Tipo_Mantenimiento>(item));
            }
            catch (Exception ex)
            {
                return BadRequest(new JsonResponse<Tipo_Mantenimiento>(null, ex.Message, ResponseStatus.error));
            }
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Update(int id, [FromBody] Tipo_Mantenimiento item)
        {
            try
            {
                if (id != item.Id)
                    return BadRequest(new JsonResponse<bool>(false, "El ID no coincide", ResponseStatus.error));

                var existingItem = await _context.TiposMantenimiento.FindAsync(id);
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
                var existingItem = await _context.TiposMantenimiento.FindAsync(id);
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
