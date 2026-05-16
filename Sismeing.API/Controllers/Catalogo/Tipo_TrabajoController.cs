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
    public class Tipo_TrabajoController : Controller
    {
        private readonly ITipo_TrabajoService _tipo_TrabajoService;

        public Tipo_TrabajoController(ITipo_TrabajoService tipo_trabajoservice)
        {
            _tipo_TrabajoService = tipo_trabajoservice;
        }

        [HttpGet]
        public async Task<ActionResult> GetAll()
        {
            try
            {
                var data = await _tipo_TrabajoService.GetAllAsync();
                return Ok(new JsonResponse<IEnumerable<Tipo_Trabajo>>(data));
            }
            catch (Exception ex)
            {
                return BadRequest(new JsonResponse<IEnumerable<Tipo_Trabajo>>(null, ex.Message, ResponseStatus.error));
            }
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult> GetById(int id)
        {
            try
            {
                var data = await _tipo_TrabajoService.GetByIdAsync(id);
                if (data == null)
                    return NotFound(new JsonResponse<Tipo_Trabajo>(null, "No encontrado", ResponseStatus.error));
                return Ok(new JsonResponse<Tipo_Trabajo>(data));
            }
            catch (Exception ex)
            {
                return BadRequest(new JsonResponse<Tipo_Trabajo>(null, ex.Message, ResponseStatus.error));
            }
        }

        [HttpPost]
        public async Task<ActionResult> Create([FromBody] Tipo_Trabajo item)
        {
            try
            {
                var userEmail = HttpContext.Items["UserEmail"]?.ToString() ?? "SYSTEM";
                var result = await _tipo_TrabajoService.CreateAsync(item, userEmail);

                return Ok(new JsonResponse<Tipo_Trabajo>(result));
            }
            catch (Exception ex)
            {
                return BadRequest(new JsonResponse<Tipo_Trabajo>(null, ex.Message, ResponseStatus.error));
            }
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Update(int id, [FromBody] Tipo_Trabajo item)
        {
            try
            {
                if (id != item.Id)
                    return BadRequest(new JsonResponse<bool>(false, "El ID no coincide", ResponseStatus.error));

                var userEmail = HttpContext.Items["UserEmail"]?.ToString() ?? "SYSTEM";
                var success = await _tipo_TrabajoService.UpdateAsync(id, item, userEmail);
                
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
                var success = await _tipo_TrabajoService.DeleteAsync(id, userEmail);

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
