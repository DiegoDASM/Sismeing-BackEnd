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
    public class EstadoController : Controller
    {
        private readonly IEstadoService _estadoService;

        public EstadoController(IEstadoService estadoservice)
        {
            _estadoService = estadoservice;
        }

        [HttpGet]
        public async Task<ActionResult> GetAll()
        {
            try
            {
                var data = await _estadoService.GetAllAsync();
                return Ok(new JsonResponse<IEnumerable<Estado>>(data));
            }
            catch (Exception ex)
            {
                return BadRequest(new JsonResponse<IEnumerable<Estado>>(null, ex.Message, ResponseStatus.error));
            }
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult> GetById(int id)
        {
            try
            {
                var data = await _estadoService.GetByIdAsync(id);
                if (data == null)
                    return NotFound(new JsonResponse<Estado>(null, "No encontrado", ResponseStatus.error));
                return Ok(new JsonResponse<Estado>(data));
            }
            catch (Exception ex)
            {
                return BadRequest(new JsonResponse<Estado>(null, ex.Message, ResponseStatus.error));
            }
        }

        [HttpPost]
        [Authorize(Roles = "Administrador,Supervisor,Tecnico,SuperAdmin")]
        public async Task<ActionResult> Create([FromBody] Estado item)
        {
            try
            {
                var userEmail = HttpContext.Items["UserEmail"]?.ToString() ?? "SYSTEM";
                var result = await _estadoService.CreateAsync(item, userEmail);

                return Ok(new JsonResponse<Estado>(result));
            }
            catch (Exception ex)
            {
                return BadRequest(new JsonResponse<Estado>(null, ex.Message, ResponseStatus.error));
            }
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "Administrador,Supervisor,Tecnico,SuperAdmin")]
        public async Task<ActionResult> Update(int id, [FromBody] Estado item)
        {
            try
            {
                if (id != item.Id)
                    return BadRequest(new JsonResponse<bool>(false, "El ID no coincide", ResponseStatus.error));

                var userEmail = HttpContext.Items["UserEmail"]?.ToString() ?? "SYSTEM";
                var success = await _estadoService.UpdateAsync(id, item, userEmail);
                
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
        [Authorize(Roles = "Administrador,Supervisor,Tecnico,SuperAdmin")]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                var userEmail = HttpContext.Items["UserEmail"]?.ToString() ?? "SYSTEM";
                var success = await _estadoService.DeleteAsync(id, userEmail);

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
