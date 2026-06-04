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
    public class FotoInstalacionController : Controller
    {
        private readonly IFoto_InstalacionService _fotoInstalacionService;
        private readonly ICloudinaryService _cloudinaryService;

        public FotoInstalacionController(IFoto_InstalacionService fotoinstalacionservice, ICloudinaryService cloudinaryService)
        {
            _fotoInstalacionService = fotoinstalacionservice;
            _cloudinaryService = cloudinaryService;
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

        [HttpGet("instalacion/{instalacionId:int}")]
        public async Task<ActionResult> GetByInstalacion(int instalacionId)
        {
            try
            {
                var data = await _fotoInstalacionService.GetByInstalacionIdAsync(instalacionId);
                return Ok(new JsonResponse<IEnumerable<Foto_Instalacion>>(data));
            }
            catch (Exception ex)
            {
                return BadRequest(new JsonResponse<IEnumerable<Foto_Instalacion>>(null, ex.Message, ResponseStatus.error));
            }
        }

        [HttpPost("instalacion/{instalacionId:int}/upload")]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult> UploadFotos(int instalacionId, List<IFormFile> files, [FromQuery] string tipo = "inicial")
        {
            try
            {
                if (files == null || files.Count == 0)
                    return BadRequest(new JsonResponse<string>(null, "No se proporcionaron archivos", ResponseStatus.error));

                var tipoValido = new[] { "inicial", "final" };
                if (!tipoValido.Contains(tipo.ToLower()))
                    return BadRequest(new JsonResponse<string>(null, "El tipo debe ser 'inicial' o 'final'", ResponseStatus.error));

                var allowed = new[] { ".jpg", ".jpeg", ".png", ".webp", ".gif" };
                var userEmail = HttpContext.Items["UserEmail"]?.ToString() ?? "SYSTEM";

                var creadas = new List<Foto_Instalacion>();

                foreach (var file in files)
                {
                    if (file.Length == 0) continue;

                    var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
                    if (!allowed.Contains(ext)) continue;

                    var fileName = $"inst_{instalacionId}_{tipo}_{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}_{Guid.NewGuid():N}{ext}";
                    var folder = $"sismeing/instalaciones/{instalacionId}";

                    using var stream = file.OpenReadStream();
                    var url = await _cloudinaryService.UploadImageAsync(stream, fileName, folder);

                    var foto = new Foto_Instalacion
                    {
                        InstalacionId = instalacionId,
                        Url = url,
                        Tipo = tipo.ToLower(),
                    };

                    var created = await _fotoInstalacionService.CreateAsync(foto, userEmail);
                    creadas.Add(created);
                }

                return Ok(new JsonResponse<IEnumerable<Foto_Instalacion>>(creadas));
            }
            catch (Exception ex)
            {
                return BadRequest(new JsonResponse<string>(null, ex.Message, ResponseStatus.error));
            }
        }
    }
}

