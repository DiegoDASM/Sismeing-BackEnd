using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sismeing.Domain.Entities.Catalogo;
using Sismeing.Service;
using Sismeing.Service.Interfaces.Catalogo;

namespace Sismeing.API.Controllers.Catalogo
{
    // Solo autenticados pueden leer roles; crearlos/editarlos/borrarlos es
    // exclusivo del SuperAdmin (los roles son a nivel de plataforma).
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class RolController : Controller
    {
        private readonly IRolService _rolService;

        public RolController(IRolService rolservice)
        {
            _rolService = rolservice;
        }

        [HttpGet]
        public async Task<ActionResult> GetAll()
        {
            try
            {
                var data = await _rolService.GetAllAsync();
                return Ok(new JsonResponse<IEnumerable<Rol>>(data));
            }
            catch (Exception ex)
            {
                return BadRequest(new JsonResponse<IEnumerable<Rol>>(null, ex.Message, ResponseStatus.error));
            }
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult> GetById(int id)
        {
            try
            {
                var data = await _rolService.GetByIdAsync(id);
                if (data == null)
                    return NotFound(new JsonResponse<Rol>(null, "No encontrado", ResponseStatus.error));
                return Ok(new JsonResponse<Rol>(data));
            }
            catch (Exception ex)
            {
                return BadRequest(new JsonResponse<Rol>(null, ex.Message, ResponseStatus.error));
            }
        }

        [HttpPost]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<ActionResult> Create([FromBody] Rol item)
        {
            try
            {
                var userEmail = HttpContext.Items["UserEmail"]?.ToString() ?? "SYSTEM";
                var result = await _rolService.CreateAsync(item, userEmail);

                return Ok(new JsonResponse<Rol>(result));
            }
            catch (Exception ex)
            {
                return BadRequest(new JsonResponse<Rol>(null, ex.Message, ResponseStatus.error));
            }
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<ActionResult> Update(int id, [FromBody] Rol item)
        {
            try
            {
                if (id != item.Id)
                    return BadRequest(new JsonResponse<bool>(false, "El ID no coincide", ResponseStatus.error));

                var userEmail = HttpContext.Items["UserEmail"]?.ToString() ?? "SYSTEM";
                var success = await _rolService.UpdateAsync(id, item, userEmail);
                
                if (!success)
                    return NotFound(new JsonResponse<bool>(false, "No encontrado", ResponseStatus.error));

                return Ok(new JsonResponse<bool>(true));
            }
            catch (Exception ex)
            {
                return BadRequest(new JsonResponse<bool>(false, ex.Message, ResponseStatus.error));
            }
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                var userEmail = HttpContext.Items["UserEmail"]?.ToString() ?? "SYSTEM";
                var success = await _rolService.DeleteAsync(id, userEmail);

                if (!success)
                    return NotFound(new JsonResponse<bool>(false, "No encontrado", ResponseStatus.error));

                return Ok(new JsonResponse<bool>(true));
            }
            catch (Exception ex)
            {
                return BadRequest(new JsonResponse<bool>(false, ex.Message, ResponseStatus.error));
            }
        }
    }
}
