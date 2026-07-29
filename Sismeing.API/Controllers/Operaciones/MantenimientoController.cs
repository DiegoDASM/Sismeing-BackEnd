using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sismeing.Domain.Entities.Operaciones;
using Sismeing.Service;
using Sismeing.Service.Interfaces.Operaciones;

namespace Sismeing.API.Controllers.Operaciones
{
    //[Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class MantenimientoController : Controller
    {
        private readonly IMantenimientoService _mantenimientoService;

        public MantenimientoController(IMantenimientoService mantenimientoservice)
        {
            _mantenimientoService = mantenimientoservice;
        }

        [HttpGet]
        public async Task<ActionResult> GetAll()
        {
            try
            {
                var data = await _mantenimientoService.GetAllAsync();
                return Ok(new JsonResponse<IEnumerable<Mantenimiento>>(data));
            }
            catch (Exception ex)
            {
                return BadRequest(new JsonResponse<IEnumerable<Mantenimiento>>(null, ex.Message, ResponseStatus.error));
            }
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult> GetById(int id)
        {
            try
            {
                var data = await _mantenimientoService.GetByIdAsync(id);
                if (data == null)
                    return NotFound(new JsonResponse<Mantenimiento>(null, "No encontrado", ResponseStatus.error));
                return Ok(new JsonResponse<Mantenimiento>(data));
            }
            catch (Exception ex)
            {
                return BadRequest(new JsonResponse<Mantenimiento>(null, ex.Message, ResponseStatus.error));
            }
        }

        [HttpPost]
        [Authorize(Policy = "Interno")]
        public async Task<ActionResult> Create([FromBody] Mantenimiento item)
        {
            try
            {
                var userEmail = HttpContext.Items["UserEmail"]?.ToString() ?? "SYSTEM";
                var result = await _mantenimientoService.CreateAsync(item, userEmail);

                return Ok(new JsonResponse<Mantenimiento>(result));
            }
            catch (Exception ex)
            {
                return BadRequest(new JsonResponse<Mantenimiento>(null, ex.GetBaseException().Message, ResponseStatus.error));
            }
        }

        [HttpPut("{id:int}")]
        [Authorize(Policy = "Gestion")]
        public async Task<ActionResult> Update(int id, [FromBody] Mantenimiento item)
        {
            try
            {
                if (id != item.Id)
                    return BadRequest(new JsonResponse<bool>(false, "El ID no coincide", ResponseStatus.error));

                var userEmail = HttpContext.Items["UserEmail"]?.ToString() ?? "SYSTEM";
                var success = await _mantenimientoService.UpdateAsync(id, item, userEmail);
                
                if (!success)
                    return NotFound(new JsonResponse<bool>(false, "No encontrado", ResponseStatus.error));

                return Ok(new JsonResponse<bool>(true));
            }
            catch (Exception ex)
            {
                return BadRequest(new JsonResponse<bool>(false, ex.GetBaseException().Message, ResponseStatus.error));
            }
        }

        [HttpDelete("{id:int}")]
        [Authorize(Policy = "Gestion")]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                var userEmail = HttpContext.Items["UserEmail"]?.ToString() ?? "SYSTEM";
                var success = await _mantenimientoService.DeleteAsync(id, userEmail);

                if (!success)
                    return NotFound(new JsonResponse<bool>(false, "No encontrado", ResponseStatus.error));

                return Ok(new JsonResponse<bool>(true));
            }
            catch (Exception ex)
            {
                return BadRequest(new JsonResponse<bool>(false, ex.Message, ResponseStatus.error));
            }
        }

        [HttpPatch("{id:int}/aprobar")]
        [Authorize(Policy = "Gestion")]
        public async Task<ActionResult> Aprobar(int id)
        {
            try
            {
                var userEmail = HttpContext.Items["UserEmail"]?.ToString() ?? "SYSTEM";
                var success = await _mantenimientoService.AprobarAsync(id, userEmail);

                if (!success)
                    return NotFound(new JsonResponse<bool>(false, "No encontrado", ResponseStatus.error));

                return Ok(new JsonResponse<bool>(true));
            }
            catch (Exception ex)
            {
                return BadRequest(new JsonResponse<bool>(false, ex.Message, ResponseStatus.error));
            }
        }

        [HttpPatch("{id:int}/activar")]
        [Authorize(Policy = "Gestion")]
        public async Task<ActionResult> Activar(int id)
        {
            try
            {
                var userEmail = HttpContext.Items["UserEmail"]?.ToString() ?? "SYSTEM";
                var success = await _mantenimientoService.ReactivarAsync(id, userEmail);

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
