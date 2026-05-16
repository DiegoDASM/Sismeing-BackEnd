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
    public class FotoMantenimientoController : Controller
    {
        private readonly IFoto_MantenimientoService _fotoMantenimientoService;

        public FotoMantenimientoController(IFoto_MantenimientoService fotomantenimientoservice)
        {
            _fotoMantenimientoService = fotomantenimientoservice;
        }

        [HttpGet]
        public async Task<ActionResult> GetAll()
        {
            try
            {
                var data = await _fotoMantenimientoService.GetAllAsync();
                return Ok(new JsonResponse<IEnumerable<Foto_Mantenimiento>>(data));
            }
            catch (Exception ex)
            {
                return BadRequest(new JsonResponse<IEnumerable<Foto_Mantenimiento>>(null, ex.Message, ResponseStatus.error));
            }
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult> GetById(int id)
        {
            try
            {
                var data = await _fotoMantenimientoService.GetByIdAsync(id);
                if (data == null)
                    return NotFound(new JsonResponse<Foto_Mantenimiento>(null, "No encontrado", ResponseStatus.error));
                return Ok(new JsonResponse<Foto_Mantenimiento>(data));
            }
            catch (Exception ex)
            {
                return BadRequest(new JsonResponse<Foto_Mantenimiento>(null, ex.Message, ResponseStatus.error));
            }
        }

        [HttpPost]
        public async Task<ActionResult> Create([FromBody] Foto_Mantenimiento item)
        {
            try
            {
                var userEmail = HttpContext.Items["UserEmail"]?.ToString() ?? "SYSTEM";
                var result = await _fotoMantenimientoService.CreateAsync(item, userEmail);

                return Ok(new JsonResponse<Foto_Mantenimiento>(result));
            }
            catch (Exception ex)
            {
                return BadRequest(new JsonResponse<Foto_Mantenimiento>(null, ex.Message, ResponseStatus.error));
            }
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Update(int id, [FromBody] Foto_Mantenimiento item)
        {
            try
            {
                if (id != item.Id)
                    return BadRequest(new JsonResponse<bool>(false, "El ID no coincide", ResponseStatus.error));

                var userEmail = HttpContext.Items["UserEmail"]?.ToString() ?? "SYSTEM";
                var success = await _fotoMantenimientoService.UpdateAsync(id, item, userEmail);
                
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
                var success = await _fotoMantenimientoService.DeleteAsync(id, userEmail);

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

