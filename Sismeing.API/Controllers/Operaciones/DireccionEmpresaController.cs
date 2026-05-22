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
    public class DireccionEmpresaController : Controller
    {
        private readonly IDireccion_EmpresaService _direccionEmpresaService;

        public DireccionEmpresaController(IDireccion_EmpresaService direccionempresaservice)
        {
            _direccionEmpresaService = direccionempresaservice;
        }

        [HttpGet("empresa/{empresaId:int}")]
        public async Task<ActionResult> GetByEmpresaId(int empresaId)
        {
            try
            {
                var data = await _direccionEmpresaService.GetByEmpresaIdAsync(empresaId);
                return Ok(new JsonResponse<IEnumerable<Direccion_Empresa>>(data));
            }
            catch (Exception ex)
            {
                return BadRequest(new JsonResponse<IEnumerable<Direccion_Empresa>>(null, ex.Message, ResponseStatus.error));
            }
        }

        [HttpGet]
        public async Task<ActionResult> GetAll()
        {
            try
            {
                var data = await _direccionEmpresaService.GetAllAsync();
                return Ok(new JsonResponse<IEnumerable<Direccion_Empresa>>(data));
            }
            catch (Exception ex)
            {
                return BadRequest(new JsonResponse<IEnumerable<Direccion_Empresa>>(null, ex.Message, ResponseStatus.error));
            }
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult> GetById(int id)
        {
            try
            {
                var data = await _direccionEmpresaService.GetByIdAsync(id);
                if (data == null)
                    return NotFound(new JsonResponse<Direccion_Empresa>(null, "No encontrado", ResponseStatus.error));
                return Ok(new JsonResponse<Direccion_Empresa>(data));
            }
            catch (Exception ex)
            {
                return BadRequest(new JsonResponse<Direccion_Empresa>(null, ex.Message, ResponseStatus.error));
            }
        }

        [HttpPost]
        public async Task<ActionResult> Create([FromBody] Direccion_Empresa item)
        {
            try
            {
                var userEmail = HttpContext.Items["UserEmail"]?.ToString() ?? "SYSTEM";
                var result = await _direccionEmpresaService.CreateAsync(item, userEmail);

                return Ok(new JsonResponse<Direccion_Empresa>(result));
            }
            catch (Exception ex)
            {
                return BadRequest(new JsonResponse<Direccion_Empresa>(null, ex.Message, ResponseStatus.error));
            }
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Update(int id, [FromBody] Direccion_Empresa item)
        {
            try
            {
                if (id != item.Id)
                    return BadRequest(new JsonResponse<bool>(false, "El ID no coincide", ResponseStatus.error));

                var userEmail = HttpContext.Items["UserEmail"]?.ToString() ?? "SYSTEM";
                var success = await _direccionEmpresaService.UpdateAsync(id, item, userEmail);
                
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
                var success = await _direccionEmpresaService.DeleteAsync(id, userEmail);

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

