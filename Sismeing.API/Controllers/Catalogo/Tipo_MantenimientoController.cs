using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sismeing.Domain.Entities.Catalogo;
using Sismeing.Service;
using Sismeing.Service.Interfaces.Catalogo;

namespace Sismeing.API.Controllers.Catalogo
{
    //[Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class Tipo_MantenimientoController : Controller
    {
        private readonly ITipo_MantenimientoService _tipo_MantenimientoService;

        public Tipo_MantenimientoController(ITipo_MantenimientoService tipo_mantenimientoservice)
        {
            _tipo_MantenimientoService = tipo_mantenimientoservice;
        }

        [HttpGet]
        public async Task<ActionResult> GetAll()
        {
            try
            {
                var data = await _tipo_MantenimientoService.GetAllAsync();
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
                var data = await _tipo_MantenimientoService.GetByIdAsync(id);
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
                var userEmail = HttpContext.Items["UserEmail"]?.ToString() ?? "SYSTEM";
                var result = await _tipo_MantenimientoService.CreateAsync(item, userEmail);

                return Ok(new JsonResponse<Tipo_Mantenimiento>(result));
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

                var userEmail = HttpContext.Items["UserEmail"]?.ToString() ?? "SYSTEM";
                var success = await _tipo_MantenimientoService.UpdateAsync(id, item, userEmail);
                
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
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                var userEmail = HttpContext.Items["UserEmail"]?.ToString() ?? "SYSTEM";
                var success = await _tipo_MantenimientoService.DeleteAsync(id, userEmail);

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
