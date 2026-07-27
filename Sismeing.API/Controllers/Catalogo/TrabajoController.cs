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
    public class TrabajoController : Controller
    {
        private readonly ITrabajoService _trabajoService;

        public TrabajoController(ITrabajoService trabajoservice)
        {
            _trabajoService = trabajoservice;
        }

        [HttpGet]
        public async Task<ActionResult> GetAll()
        {
            try
            {
                var data = await _trabajoService.GetAllAsync();
                return Ok(new JsonResponse<IEnumerable<Trabajo>>(data));
            }
            catch (Exception ex)
            {
                return BadRequest(new JsonResponse<IEnumerable<Trabajo>>(null, ex.Message, ResponseStatus.error));
            }
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult> GetById(int id)
        {
            try
            {
                var data = await _trabajoService.GetByIdAsync(id);
                if (data == null)
                    return NotFound(new JsonResponse<Trabajo>(null, "No encontrado", ResponseStatus.error));
                return Ok(new JsonResponse<Trabajo>(data));
            }
            catch (Exception ex)
            {
                return BadRequest(new JsonResponse<Trabajo>(null, ex.Message, ResponseStatus.error));
            }
        }

        [HttpGet("mantenimiento/{mantenimientoId:int}")]
        public async Task<ActionResult> GetByMantenimiento(int mantenimientoId)
        {
            try
            {
                var data = await _trabajoService.GetByMantenimientoAsync(mantenimientoId);
                return Ok(new JsonResponse<IEnumerable<Trabajo>>(data));
            }
            catch (Exception ex)
            {
                return BadRequest(new JsonResponse<IEnumerable<Trabajo>>(null, ex.Message, ResponseStatus.error));
            }
        }

        [HttpPost]
        [Authorize(Roles = "Administrador,Supervisor,Tecnico,SuperAdmin")]
        public async Task<ActionResult> Create([FromBody] Trabajo item)
        {
            try
            {
                var userEmail = HttpContext.Items["UserEmail"]?.ToString() ?? "SYSTEM";
                var result = await _trabajoService.CreateAsync(item, userEmail);

                return Ok(new JsonResponse<Trabajo>(result));
            }
            catch (Exception ex)
            {
                return BadRequest(new JsonResponse<Trabajo>(null, ex.Message, ResponseStatus.error));
            }
        }

        [HttpPost("mantenimiento/{mantenimientoId:int}")]
        public async Task<ActionResult> ReemplazarPorMantenimiento(int mantenimientoId, [FromBody] List<Trabajo> items)
        {
            try
            {
                var userEmail = HttpContext.Items["UserEmail"]?.ToString() ?? "SYSTEM";
                var result = await _trabajoService.ReemplazarPorMantenimientoAsync(mantenimientoId, items ?? new List<Trabajo>(), userEmail);
                return Ok(new JsonResponse<IEnumerable<Trabajo>>(result));
            }
            catch (Exception ex)
            {
                return BadRequest(new JsonResponse<IEnumerable<Trabajo>>(null, ex.Message, ResponseStatus.error));
            }
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "Administrador,Supervisor,Tecnico,SuperAdmin")]
        public async Task<ActionResult> Update(int id, [FromBody] Trabajo item)
        {
            try
            {
                if (id != item.Id)
                    return BadRequest(new JsonResponse<bool>(false, "El ID no coincide", ResponseStatus.error));

                var userEmail = HttpContext.Items["UserEmail"]?.ToString() ?? "SYSTEM";
                var success = await _trabajoService.UpdateAsync(id, item, userEmail);
                
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
                var success = await _trabajoService.DeleteAsync(id, userEmail);

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
