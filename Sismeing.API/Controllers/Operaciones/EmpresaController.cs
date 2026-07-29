using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sismeing.Domain.Entities.Operaciones;
using Sismeing.Service;
using Sismeing.Service.Interfaces.Comunes;
using Sismeing.Service.Interfaces.Operaciones;

namespace Sismeing.API.Controllers.Operaciones
{
    //[Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class EmpresaController : Controller
    {
        private readonly IEmpresaService _empresaService;
        private readonly ICloudinaryService _cloudinaryService;

        public EmpresaController(IEmpresaService empresaservice, ICloudinaryService cloudinaryService)
        {
            _empresaService = empresaservice;
            _cloudinaryService = cloudinaryService;
        }

        [HttpGet]
        public async Task<ActionResult> GetAll()
        {
            try
            {
                var data = await _empresaService.GetAllAsync();
                return Ok(new JsonResponse<IEnumerable<Empresa>>(data));
            }
            catch (Exception ex)
            {
                return BadRequest(new JsonResponse<IEnumerable<Empresa>>(null, ex.Message, ResponseStatus.error));
            }
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult> GetById(int id)
        {
            try
            {
                var data = await _empresaService.GetByIdAsync(id);
                if (data == null)
                    return NotFound(new JsonResponse<Empresa>(null, "No encontrado", ResponseStatus.error));
                return Ok(new JsonResponse<Empresa>(data));
            }
            catch (Exception ex)
            {
                return BadRequest(new JsonResponse<Empresa>(null, ex.Message, ResponseStatus.error));
            }
        }

        [HttpPost]
        [Authorize(Policy = "Gestion")]
        public async Task<ActionResult> Create([FromBody] Empresa item)
        {
            try
            {
                var userEmail = HttpContext.Items["UserEmail"]?.ToString() ?? "SYSTEM";
                var result = await _empresaService.CreateAsync(item, userEmail);

                return Ok(new JsonResponse<Empresa>(result));
            }
            catch (Exception ex)
            {
                return BadRequest(new JsonResponse<Empresa>(null, ex.Message, ResponseStatus.error));
            }
        }

        [HttpPut("{id:int}")]
        [Authorize(Policy = "Gestion")]
        public async Task<ActionResult> Update(int id, [FromBody] Empresa item)
        {
            try
            {
                if (id != item.Id)
                    return BadRequest(new JsonResponse<bool>(false, "El ID no coincide", ResponseStatus.error));

                var userEmail = HttpContext.Items["UserEmail"]?.ToString() ?? "SYSTEM";
                var success = await _empresaService.UpdateAsync(id, item, userEmail);
                
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
                var success = await _empresaService.DeleteAsync(id, userEmail);

                if (!success)
                    return NotFound(new JsonResponse<bool>(false, "No encontrado", ResponseStatus.error));

                return Ok(new JsonResponse<bool>(true));
            }
            catch (Exception ex)
            {
                return BadRequest(new JsonResponse<bool>(false, ex.Message, ResponseStatus.error));
            }
        }

        [HttpPost("{id:int}/upload-logo")]
        [Authorize(Policy = "Gestion")]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult> UploadLogo(int id, IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                    return BadRequest(new JsonResponse<string>(null, "No se proporcionó archivo", ResponseStatus.error));

                var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
                var allowed = new[] { ".jpg", ".jpeg", ".png", ".webp", ".gif" };
                if (!allowed.Contains(ext))
                    return BadRequest(new JsonResponse<string>(null, "Formato no válido. Use JPG, PNG, WEBP o GIF", ResponseStatus.error));

                var fileName = $"empresa_{id}_{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}{ext}";
                using var stream = file.OpenReadStream();
                var logoUrl = await _cloudinaryService.UploadImageAsync(stream, fileName, "sismeing/logos");

                var result = await _empresaService.UpdateLogoAsync(id, logoUrl);

                if (result == null)
                    return NotFound(new JsonResponse<string>(null, "Empresa no encontrada", ResponseStatus.error));

                return Ok(new JsonResponse<string>(logoUrl));
            }
            catch (Exception ex)
            {
                return BadRequest(new JsonResponse<string>(null, ex.Message, ResponseStatus.error));
            }
        }

        [HttpPatch("{id:int}/activar")]
        [Authorize(Policy = "Gestion")]
        public async Task<ActionResult> Activar(int id)
        {
            try
            {
                var userEmail = HttpContext.Items["UserEmail"]?.ToString() ?? "SYSTEM";
                var success = await _empresaService.ReactivarAsync(id, userEmail);

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
