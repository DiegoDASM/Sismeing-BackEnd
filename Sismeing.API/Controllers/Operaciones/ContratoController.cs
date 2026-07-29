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
    public class ContratoController : Controller
    {
        private readonly IContratoService _contratoService;

        public ContratoController(IContratoService contratoservice)
        {
            _contratoService = contratoservice;
        }

        [HttpGet]
        public async Task<ActionResult> GetAll()
        {
            try
            {
                var data = await _contratoService.GetAllAsync();
                return Ok(new JsonResponse<IEnumerable<Contrato>>(data));
            }
            catch (Exception ex)
            {
                return BadRequest(new JsonResponse<IEnumerable<Contrato>>(null, ex.Message, ResponseStatus.error));
            }
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult> GetById(int id)
        {
            try
            {
                var data = await _contratoService.GetByIdAsync(id);
                if (data == null)
                    return NotFound(new JsonResponse<Contrato>(null, "No encontrado", ResponseStatus.error));
                return Ok(new JsonResponse<Contrato>(data));
            }
            catch (Exception ex)
            {
                return BadRequest(new JsonResponse<Contrato>(null, ex.Message, ResponseStatus.error));
            }
        }

        [HttpPost]
        [Authorize(Policy = "Gestion")]
        public async Task<ActionResult> Create([FromBody] Contrato item)
        {
            try
            {
                var userEmail = HttpContext.Items["UserEmail"]?.ToString() ?? "SYSTEM";
                var result = await _contratoService.CreateAsync(item, userEmail);

                return Ok(new JsonResponse<Contrato>(result));
            }
            catch (Exception ex)
            {
                return BadRequest(new JsonResponse<Contrato>(null, ex.Message, ResponseStatus.error));
            }
        }

        [HttpPut("{id:int}")]
        [Authorize(Policy = "Gestion")]
        public async Task<ActionResult> Update(int id, [FromBody] Contrato item)
        {
            try
            {
                if (id != item.Id)
                    return BadRequest(new JsonResponse<bool>(false, "El ID no coincide", ResponseStatus.error));

                var userEmail = HttpContext.Items["UserEmail"]?.ToString() ?? "SYSTEM";
                var success = await _contratoService.UpdateAsync(id, item, userEmail);
                
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
        [Authorize(Policy = "Gestion")]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                var userEmail = HttpContext.Items["UserEmail"]?.ToString() ?? "SYSTEM";
                var success = await _contratoService.DeleteAsync(id, userEmail);

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
                var success = await _contratoService.ReactivarAsync(id, userEmail);

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
