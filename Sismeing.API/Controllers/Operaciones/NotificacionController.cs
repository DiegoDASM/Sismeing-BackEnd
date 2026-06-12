using Microsoft.AspNetCore.Mvc;
using Sismeing.Domain.Entities.Operaciones;
using Sismeing.Service;
using Sismeing.Service.Interfaces.Operaciones;

namespace Sismeing.API.Controllers.Operaciones
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificacionController : Controller
    {
        private readonly INotificacionService _notificacionService;

        public NotificacionController(INotificacionService notificacionService)
        {
            _notificacionService = notificacionService;
        }

        [HttpGet("usuario/{usuarioId:int}")]
        public async Task<ActionResult> GetByUsuario(int usuarioId)
        {
            try
            {
                var data = await _notificacionService.GetByUsuarioAsync(usuarioId);
                return Ok(new JsonResponse<IEnumerable<Notificacion>>(data));
            }
            catch (Exception ex)
            {
                return BadRequest(new JsonResponse<IEnumerable<Notificacion>>(null, ex.Message, ResponseStatus.error));
            }
        }

        [HttpGet("usuario/{usuarioId:int}/no-leidas")]
        public async Task<ActionResult> GetNoLeidasCount(int usuarioId)
        {
            try
            {
                var count = await _notificacionService.GetNoLeidasCountAsync(usuarioId);
                return Ok(new JsonResponse<int>(count));
            }
            catch (Exception ex)
            {
                return BadRequest(new JsonResponse<int>(0, ex.Message, ResponseStatus.error));
            }
        }

        [HttpPost]
        public async Task<ActionResult> Create([FromBody] Notificacion item)
        {
            try
            {
                var userEmail = HttpContext.Items["UserEmail"]?.ToString() ?? "SYSTEM";
                var result = await _notificacionService.CreateAsync(item, userEmail);

                return Ok(new JsonResponse<Notificacion>(result));
            }
            catch (Exception ex)
            {
                return BadRequest(new JsonResponse<Notificacion>(null, ex.Message, ResponseStatus.error));
            }
        }

        [HttpPatch("{id:int}/leida")]
        public async Task<ActionResult> MarcarLeida(int id)
        {
            try
            {
                var userEmail = HttpContext.Items["UserEmail"]?.ToString() ?? "SYSTEM";
                var success = await _notificacionService.MarcarLeidaAsync(id, userEmail);

                if (!success)
                    return NotFound(new JsonResponse<bool>(false, "No encontrado", ResponseStatus.error));

                return Ok(new JsonResponse<bool>(true));
            }
            catch (Exception ex)
            {
                return BadRequest(new JsonResponse<bool>(false, ex.Message, ResponseStatus.error));
            }
        }

        [HttpPatch("usuario/{usuarioId:int}/leidas")]
        public async Task<ActionResult> MarcarTodasLeidas(int usuarioId)
        {
            try
            {
                var userEmail = HttpContext.Items["UserEmail"]?.ToString() ?? "SYSTEM";
                var count = await _notificacionService.MarcarTodasLeidasAsync(usuarioId, userEmail);

                return Ok(new JsonResponse<int>(count));
            }
            catch (Exception ex)
            {
                return BadRequest(new JsonResponse<int>(0, ex.Message, ResponseStatus.error));
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                var userEmail = HttpContext.Items["UserEmail"]?.ToString() ?? "SYSTEM";
                var success = await _notificacionService.DeleteAsync(id, userEmail);

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
