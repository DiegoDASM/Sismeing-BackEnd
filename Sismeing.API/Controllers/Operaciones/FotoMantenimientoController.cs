using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sismeing.Domain.Entities.Operaciones;
using Sismeing.Service;
using Sismeing.Service.Interfaces.Catalogo;
using Sismeing.Service.Interfaces.Comunes;
using Sismeing.Service.Interfaces.Operaciones;

namespace Sismeing.API.Controllers.Operaciones
{
    //[Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class FotoMantenimientoController : Controller
    {
        private readonly IFoto_MantenimientoService _fotoMantenimientoService;
        private readonly ICloudinaryService _cloudinaryService;
        private readonly ITrabajoService _trabajoService;

        public FotoMantenimientoController(IFoto_MantenimientoService fotomantenimientoservice, ICloudinaryService cloudinaryService, ITrabajoService trabajoService)
        {
            _fotoMantenimientoService = fotomantenimientoservice;
            _cloudinaryService = cloudinaryService;
            _trabajoService = trabajoService;
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
        [Authorize(Policy = "Interno")]
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
        [Authorize(Policy = "Interno")]
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
        [Authorize(Policy = "Interno")]
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

        [HttpGet("mantenimiento/{mantenimientoId:int}")]
        public async Task<ActionResult> GetByMantenimiento(int mantenimientoId)
        {
            try
            {
                var data = await _fotoMantenimientoService.GetByMantenimientoIdAsync(mantenimientoId);
                return Ok(new JsonResponse<IEnumerable<Foto_Mantenimiento>>(data));
            }
            catch (Exception ex)
            {
                return BadRequest(new JsonResponse<IEnumerable<Foto_Mantenimiento>>(null, ex.Message, ResponseStatus.error));
            }
        }

        [HttpPost("mantenimiento/{mantenimientoId:int}/upload")]
        [Authorize(Policy = "Interno")]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult> UploadFotos(int mantenimientoId, List<IFormFile> files,
            [FromQuery] string tipo = "inicial", [FromQuery] int? trabajoId = null)
        {
            try
            {
                if (files == null || files.Count == 0)
                    return BadRequest(new JsonResponse<string>(null, "No se proporcionaron archivos", ResponseStatus.error));

                var tipoValido = new[] { "inicial", "final" };
                if (!tipoValido.Contains(tipo.ToLower()))
                    return BadRequest(new JsonResponse<string>(null, "El tipo debe ser 'inicial' o 'final'", ResponseStatus.error));

                // La foto solo puede ligarse a un trabajo vigente de ESTE mantenimiento.
                if (trabajoId.HasValue)
                {
                    var trabajo = await _trabajoService.GetByIdAsync(trabajoId.Value);
                    if (trabajo == null || !trabajo.Activo || trabajo.MantenimientoId != mantenimientoId)
                        return BadRequest(new JsonResponse<string>(null, "El trabajo indicado no pertenece a este mantenimiento", ResponseStatus.error));
                }

                var allowed = new[] { ".jpg", ".jpeg", ".png", ".webp", ".gif" };
                var userEmail = HttpContext.Items["UserEmail"]?.ToString() ?? "SYSTEM";

                var creadas = new List<Foto_Mantenimiento>();

                foreach (var file in files)
                {
                    if (file.Length == 0) continue;

                    var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
                    if (!allowed.Contains(ext)) continue;

                    var fileName = $"mant_{mantenimientoId}_{tipo}_{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}_{Guid.NewGuid():N}{ext}";
                    var folder = $"sismeing/mantenimientos/{mantenimientoId}";

                    using var stream = file.OpenReadStream();
                    var url = await _cloudinaryService.UploadImageAsync(stream, fileName, folder);

                    var foto = new Foto_Mantenimiento
                    {
                        MantenimientoId = mantenimientoId,
                        Url = url,
                        Tipo = tipo.ToLower(),
                        // Trabajo al que corresponde la evidencia (null = foto general).
                        TrabajoId = trabajoId,
                    };

                    var created = await _fotoMantenimientoService.CreateAsync(foto, userEmail);
                    creadas.Add(created);
                }

                return Ok(new JsonResponse<IEnumerable<Foto_Mantenimiento>>(creadas));
            }
            catch (Exception ex)
            {
                return BadRequest(new JsonResponse<string>(null, ex.Message, ResponseStatus.error));
            }
        }
    }
}

