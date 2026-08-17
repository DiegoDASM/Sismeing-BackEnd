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
    public class InstalacionController : Controller
    {
        private readonly IInstalacionService _instalacionService;

        public InstalacionController(IInstalacionService instalacionService)
        {
            _instalacionService = instalacionService;
        }

        [HttpGet]
        public async Task<ActionResult> GetAll()
        {
            try
            {
                var data = await _instalacionService.GetAllAsync();
                return Ok(new JsonResponse<IEnumerable<Instalacion>>(data));
            }
            catch (Exception ex)
            {
                return BadRequest(new JsonResponse<IEnumerable<Instalacion>>(null, ex.Message, ResponseStatus.error));
            }
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult> GetById(int id)
        {
            try
            {
                var data = await _instalacionService.GetByIdAsync(id);
                if (data == null)
                    return NotFound(new JsonResponse<Instalacion>(null, "No encontrado", ResponseStatus.error));
                return Ok(new JsonResponse<Instalacion>(data));
            }
            catch (Exception ex)
            {
                return BadRequest(new JsonResponse<Instalacion>(null, ex.Message, ResponseStatus.error));
            }
        }

        [HttpPost]
        [Authorize(Policy = "Interno")]
        public async Task<ActionResult> Create([FromBody] Instalacion item)
        {
            try
            {
                var userEmail = HttpContext.Items["UserEmail"]?.ToString() ?? "SYSTEM";
                var result = await _instalacionService.CreateAsync(item, userEmail);

                return Ok(new JsonResponse<Instalacion>(result));
            }
            catch (Exception ex)
            {
                return BadRequest(new JsonResponse<Instalacion>(null, ex.Message, ResponseStatus.error));
            }
        }

        [HttpPut("{id:int}")]
        [Authorize(Policy = "Gestion")]
        public async Task<ActionResult> Update(int id, [FromBody] Instalacion item)
        {
            try
            {
                if (id != item.Id)
                    return BadRequest(new JsonResponse<bool>(false, "El ID no coincide", ResponseStatus.error));

                var userEmail = HttpContext.Items["UserEmail"]?.ToString() ?? "SYSTEM";
                var success = await _instalacionService.UpdateAsync(id, item, userEmail);
                
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
                var success = await _instalacionService.DeleteAsync(id, userEmail);

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
        [Authorize(Policy = "Aprobacion")]
        public async Task<ActionResult> Aprobar(int id)
        {
            try
            {
                var userEmail = HttpContext.Items["UserEmail"]?.ToString() ?? "SYSTEM";
                var success = await _instalacionService.AprobarAsync(id, userEmail);

                if (!success)
                    return NotFound(new JsonResponse<bool>(false, "No encontrado", ResponseStatus.error));

                return Ok(new JsonResponse<bool>(true));
            }
            catch (Exception ex)
            {
                return BadRequest(new JsonResponse<bool>(false, ex.Message, ResponseStatus.error));
            }
        }

        // Segunda aprobación (la del cliente de la empresa). Los administradores
        // también pueden darla en nombre del cliente; el Supervisor no.
        [HttpPatch("{id:int}/aprobar-cliente")]
        [Authorize(Roles = "Cliente,Administrador,SuperAdmin")]
        public async Task<ActionResult> AprobarCliente(int id)
        {
            try
            {
                var userEmail = HttpContext.Items["UserEmail"]?.ToString() ?? "SYSTEM";
                var success = await _instalacionService.AprobarClienteAsync(id, userEmail);

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
                var success = await _instalacionService.ReactivarAsync(id, userEmail);

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
