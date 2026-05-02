using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sismeing.Domain.Entities.Operaciones;
using Sismeing.Infrestructura.Persistence;
using Sismeing.Service;
using Sismeing.Service.Interfaces.Operaciones;

namespace Sismeing.API.Controllers.Operaciones
{
    //[Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class Area_EmpresaController : Controller
    {
        private readonly SupaBaseDBcontext _context;

        public Area_EmpresaController(SupaBaseDBcontext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult> GetAll()
        {
            try
            {
                var data = await _context.AreasEmpresa.ToListAsync();
                return Ok(new JsonResponse<IEnumerable<Area_Empresa>>(data));
            }
            catch (Exception ex)
            {
                return BadRequest(new JsonResponse<IEnumerable<Area_Empresa>>(null, ex.Message, ResponseStatus.error));
            }
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult> GetById(int id)
        {
            try
            {
                // CAMBIO AQUÍ: Buscar directo en la DB
                var data = await _context.AreasEmpresa.FindAsync(id);

                if (data == null)
                    return NotFound(new JsonResponse<Area_Empresa>(null, "No encontrado", ResponseStatus.error));

                return Ok(new JsonResponse<Area_Empresa>(data));
            }
            catch (Exception ex)
            {
                return BadRequest(new JsonResponse<Area_Empresa>(null, ex.Message, ResponseStatus.error));
            }
        }


        [HttpPost]
        public async Task<ActionResult> Create([FromBody] Area_Empresa item)
        {
            try
            {
                item.UsuarioRegistro = HttpContext.Items["UserEmail"]?.ToString() ?? "SYSTEM";
                item.FechaRegistro = DateTime.UtcNow;
                item.Activo = true;

                // CAMBIO AQUÍ: Añadir a la tabla y guardar
                _context.AreasEmpresa.Add(item);
                await _context.SaveChangesAsync();

                return Ok(new JsonResponse<Area_Empresa>(item));
            }
            catch (Exception ex)
            {
                return BadRequest(new JsonResponse<Area_Empresa>(null, ex.Message, ResponseStatus.error));
            }
        }


        [HttpPut("{id:int}")]
        public async Task<ActionResult> Update(int id, [FromBody] Area_Empresa item)
        {
            try
            {
                if (id != item.Id)
                    return BadRequest(new JsonResponse<bool>(false, "El ID no coincide", ResponseStatus.error));

                // CAMBIO AQUÍ: Buscar, actualizar y guardar
                var existingItem = await _context.AreasEmpresa.FindAsync(id);
                if (existingItem == null)
                    return NotFound(new JsonResponse<bool>(false, "No encontrado", ResponseStatus.error));

                // Copiar los valores nuevos al objeto existente
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
                // CAMBIO AQUÍ: Buscar, cambiar estado y guardar
                var existingItem = await _context.AreasEmpresa.FindAsync(id);
                if (existingItem == null)
                    return NotFound(new JsonResponse<bool>(false, "No encontrado", ResponseStatus.error));

                existingItem.Activo = false; // Eliminación lógica
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

    }
}
