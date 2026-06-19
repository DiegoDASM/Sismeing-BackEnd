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
    public class MedicionController : Controller
    {
        private readonly IMedicionService _medicionService;

        public MedicionController(IMedicionService medicionservice)
        {
            _medicionService = medicionservice;
        }

        [HttpGet]
        public async Task<ActionResult> GetAll()
        {
            try
            {
                var data = await _medicionService.GetAllAsync();
                return Ok(new JsonResponse<IEnumerable<Medicion>>(data));
            }
            catch (Exception ex)
            {
                return BadRequest(new JsonResponse<IEnumerable<Medicion>>(null, ex.Message, ResponseStatus.error));
            }
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult> GetById(int id)
        {
            try
            {
                var data = await _medicionService.GetByIdAsync(id);
                if (data == null)
                    return NotFound(new JsonResponse<Medicion>(null, "No encontrado", ResponseStatus.error));
                return Ok(new JsonResponse<Medicion>(data));
            }
            catch (Exception ex)
            {
                return BadRequest(new JsonResponse<Medicion>(null, ex.Message, ResponseStatus.error));
            }
        }

        [HttpGet("informe/{informeId:int}/equipo/{equipoId:int}")]
        public async Task<ActionResult> GetByInforme(int informeId, int equipoId)
        {
            try
            {
                var data = await _medicionService.GetByInformeAsync(informeId, equipoId);
                return Ok(new JsonResponse<IEnumerable<Medicion>>(data));
            }
            catch (Exception ex)
            {
                return BadRequest(new JsonResponse<IEnumerable<Medicion>>(null, ex.Message, ResponseStatus.error));
            }
        }

        [HttpPost]
        public async Task<ActionResult> Create([FromBody] Medicion item)
        {
            try
            {
                var userEmail = HttpContext.Items["UserEmail"]?.ToString() ?? "SYSTEM";
                var result = await _medicionService.CreateAsync(item, userEmail);

                return Ok(new JsonResponse<Medicion>(result));
            }
            catch (Exception ex)
            {
                return BadRequest(new JsonResponse<Medicion>(null, ex.Message, ResponseStatus.error));
            }
        }

        [HttpPost("batch")]
        public async Task<ActionResult> CreateBatch([FromBody] List<Medicion> items)
        {
            try
            {
                if (items == null || items.Count == 0)
                    return BadRequest(new JsonResponse<IEnumerable<Medicion>>(null, "No se proporcionaron mediciones", ResponseStatus.error));

                var userEmail = HttpContext.Items["UserEmail"]?.ToString() ?? "SYSTEM";
                var result = await _medicionService.CreateBatchAsync(items, userEmail);
                return Ok(new JsonResponse<IEnumerable<Medicion>>(result));
            }
            catch (Exception ex)
            {
                return BadRequest(new JsonResponse<IEnumerable<Medicion>>(null, ex.Message, ResponseStatus.error));
            }
        }

        [HttpPost("informe/{informeId:int}/equipo/{equipoId:int}")]
        public async Task<ActionResult> ReemplazarPorInforme(int informeId, int equipoId, [FromBody] List<Medicion> items)
        {
            try
            {
                var userEmail = HttpContext.Items["UserEmail"]?.ToString() ?? "SYSTEM";
                var result = await _medicionService.ReemplazarPorInformeAsync(informeId, equipoId, items ?? new List<Medicion>(), userEmail);
                return Ok(new JsonResponse<IEnumerable<Medicion>>(result));
            }
            catch (Exception ex)
            {
                return BadRequest(new JsonResponse<IEnumerable<Medicion>>(null, ex.Message, ResponseStatus.error));
            }
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Update(int id, [FromBody] Medicion item)
        {
            try
            {
                if (id != item.Id)
                    return BadRequest(new JsonResponse<bool>(false, "El ID no coincide", ResponseStatus.error));

                var userEmail = HttpContext.Items["UserEmail"]?.ToString() ?? "SYSTEM";
                var success = await _medicionService.UpdateAsync(id, item, userEmail);
                
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
                var success = await _medicionService.DeleteAsync(id, userEmail);

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
