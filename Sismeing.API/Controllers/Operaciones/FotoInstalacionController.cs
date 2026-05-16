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
    public class FotoInstalacionController : Controller
    {
        private readonly IFoto_InstalacionService _fotoInstalacionService;

        public FotoInstalacionController(IFoto_InstalacionService fotoinstalacionservice)
        {
            _fotoInstalacionService = fotoinstalacionservice;
        }

        [HttpGet]
        public async Task<ActionResult> GetAll()
        {
            try
            {
                var data = await _fotoInstalacionService.GetAllAsync();
                return Ok(new JsonResponse<IEnumerable<Foto_Instalacion>>(data));
            }
            catch (Exception ex)
            {
                return BadRequest(new JsonResponse<IEnumerable<Foto_Instalacion>>(null, ex.Message, ResponseStatus.error));
            }
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult> GetById(int id)
        {
            try
            {
                var data = await _fotoInstalacionService.GetByIdAsync(id);
                if (data == null)
                    return NotFound(new JsonResponse<Foto_Instalacion>(null, "No encontrado", ResponseStatus.error));
                return Ok(new JsonResponse<Foto_Instalacion>(data));
            }
            catch (Exception ex)
            {
                return BadRequest(new JsonResponse<Foto_Instalacion>(null, ex.Message, ResponseStatus.error));
            }
        }

        [HttpPost]
        public async Task<ActionResult> Create([FromBody] Foto_Instalacion item)
        {
            try
            {
                var userEmail = HttpContext.Items["UserEmail"]?.ToString() ?? "SYSTEM";
                var result = await _fotoInstalacionService.CreateAsync(item, userEmail);

                return Ok(new JsonResponse<Foto_Instalacion>(result));
            }
            catch (Exception ex)
            {
                return BadRequest(new JsonResponse<Foto_Instalacion>(null, ex.Message, ResponseStatus.error));
            }
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Update(int id, [FromBody] Foto_Instalacion item)
        {
            try
            {
                if (id != item.Id)
                    return BadRequest(new JsonResponse<bool>(false, "El ID no coincide", ResponseStatus.error));

                var userEmail = HttpContext.Items["UserEmail"]?.ToString() ?? "SYSTEM";
                var success = await _fotoInstalacionService.UpdateAsync(id, item, userEmail);
                
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
                var success = await _fotoInstalacionService.DeleteAsync(id, userEmail);

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

