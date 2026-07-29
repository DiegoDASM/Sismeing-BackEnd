using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sismeing.Domain.Entities.Operaciones;
using Sismeing.Service;
using Sismeing.Service.Interfaces.Comunes;
using Sismeing.Service.Interfaces.Operaciones;

namespace Sismeing.API.Controllers.Operaciones
{
    [ApiController]
    [Route("api/[controller]")]
    public class FotoVisitaTecnicaController : Controller
    {
        private readonly IFoto_VisitaTecnicaService _fotoVisitaService;
        private readonly ICloudinaryService _cloudinaryService;

        public FotoVisitaTecnicaController(IFoto_VisitaTecnicaService fotoVisitaService, ICloudinaryService cloudinaryService)
        {
            _fotoVisitaService = fotoVisitaService;
            _cloudinaryService = cloudinaryService;
        }

        [HttpGet]
        public async Task<ActionResult> GetAll()
        {
            try
            {
                var data = await _fotoVisitaService.GetAllAsync();
                return Ok(new JsonResponse<IEnumerable<Foto_VisitaTecnica>>(data));
            }
            catch (Exception ex)
            {
                return BadRequest(new JsonResponse<IEnumerable<Foto_VisitaTecnica>>(null, ex.Message, ResponseStatus.error));
            }
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult> GetById(int id)
        {
            try
            {
                var data = await _fotoVisitaService.GetByIdAsync(id);
                if (data == null)
                    return NotFound(new JsonResponse<Foto_VisitaTecnica>(null, "No encontrado", ResponseStatus.error));
                return Ok(new JsonResponse<Foto_VisitaTecnica>(data));
            }
            catch (Exception ex)
            {
                return BadRequest(new JsonResponse<Foto_VisitaTecnica>(null, ex.Message, ResponseStatus.error));
            }
        }

        [HttpDelete("{id:int}")]
        [Authorize(Policy = "Interno")]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                var userEmail = HttpContext.Items["UserEmail"]?.ToString() ?? "SYSTEM";
                var success = await _fotoVisitaService.DeleteAsync(id, userEmail);

                if (!success)
                    return NotFound(new JsonResponse<bool>(false, "No encontrado", ResponseStatus.error));

                return Ok(new JsonResponse<bool>(true));
            }
            catch (Exception ex)
            {
                return BadRequest(new JsonResponse<bool>(false, ex.Message, ResponseStatus.error));
            }
        }

        [HttpGet("visita/{visitaTecnicaId:int}")]
        public async Task<ActionResult> GetByVisita(int visitaTecnicaId)
        {
            try
            {
                var data = await _fotoVisitaService.GetByVisitaTecnicaIdAsync(visitaTecnicaId);
                return Ok(new JsonResponse<IEnumerable<Foto_VisitaTecnica>>(data));
            }
            catch (Exception ex)
            {
                return BadRequest(new JsonResponse<IEnumerable<Foto_VisitaTecnica>>(null, ex.Message, ResponseStatus.error));
            }
        }

        [HttpPost("visita/{visitaTecnicaId:int}/upload")]
        [Authorize(Policy = "Interno")]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult> UploadFotos(int visitaTecnicaId, List<IFormFile> files, [FromQuery] string tipo = "inicial")
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

                var creadas = new List<Foto_VisitaTecnica>();

                foreach (var file in files)
                {
                    if (file.Length == 0) continue;

                    var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
                    if (!allowed.Contains(ext)) continue;

                    var fileName = $"visita_{visitaTecnicaId}_{tipo}_{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}_{Guid.NewGuid():N}{ext}";
                    var folder = $"sismeing/visitas/{visitaTecnicaId}";

                    using var stream = file.OpenReadStream();
                    var url = await _cloudinaryService.UploadImageAsync(stream, fileName, folder);

                    var foto = new Foto_VisitaTecnica
                    {
                        VisitaTecnicaId = visitaTecnicaId,
                        Url = url,
                        Tipo = tipo.ToLower(),
                    };

                    var created = await _fotoVisitaService.CreateAsync(foto, userEmail);
                    creadas.Add(created);
                }

                return Ok(new JsonResponse<IEnumerable<Foto_VisitaTecnica>>(creadas));
            }
            catch (Exception ex)
            {
                return BadRequest(new JsonResponse<string>(null, ex.Message, ResponseStatus.error));
            }
        }
    }
}
