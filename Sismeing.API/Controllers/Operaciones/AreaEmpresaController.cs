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
    public class Area_EmpresaController : Controller
    {
        private readonly IArea_EmpresaService _areaEmpresaService;

        public Area_EmpresaController(IArea_EmpresaService areaempresaservice)
        {
            _areaEmpresaService = areaempresaservice;
        }

        [HttpGet]
        public async Task<ActionResult> GetAll()
        {
            try
            {
                var data = await _areaEmpresaService.GetAllAsync();
                return Ok(new JsonResponse<IEnumerable<Area_Empresa>>(data));
            }
            catch (Exception ex)
            {
                return BadRequest(new JsonResponse<IEnumerable<Area_Empresa>>(null, ex.Message, ResponseStatus.error));
            }
        }

        [HttpGet("empresa/{empresaId:int}")]
        public async Task<ActionResult> GetByEmpresaId(int empresaId)
        {
            try
            {
                var data = await _areaEmpresaService.GetByEmpresaIdAsync(empresaId);
                return Ok(new JsonResponse<IEnumerable<Area_Empresa>>(data));
            }
            catch (Exception ex)
            {
                return BadRequest(new JsonResponse<IEnumerable<Area_Empresa>>(null, ex.Message, ResponseStatus.error));
            }
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult> GetById(int id)
        {
            try
            {
                var data = await _areaEmpresaService.GetByIdAsync(id);
                if (data == null)
                    return NotFound(new JsonResponse<Area_Empresa>(null, "No encontrado", ResponseStatus.error));
                return Ok(new JsonResponse<Area_Empresa>(data));
            }
            catch (Exception ex)
            {
                return BadRequest(new JsonResponse<Area_Empresa>(null, ex.Message, ResponseStatus.error));
            }
        }

        [HttpPost]
        public async Task<ActionResult> Create([FromBody] Area_Empresa item)
        {
            try
            {
                var userEmail = HttpContext.Items["UserEmail"]?.ToString() ?? "SYSTEM";
                var result = await _areaEmpresaService.CreateAsync(item, userEmail);

                return Ok(new JsonResponse<Area_Empresa>(result));
            }
            catch (Exception ex)
            {
                return BadRequest(new JsonResponse<Area_Empresa>(null, ex.Message, ResponseStatus.error));
            }
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Update(int id, [FromBody] Area_Empresa item)
        {
            try
            {
                if (id != item.Id)
                    return BadRequest(new JsonResponse<bool>(false, "El ID no coincide", ResponseStatus.error));

                var userEmail = HttpContext.Items["UserEmail"]?.ToString() ?? "SYSTEM";
                var success = await _areaEmpresaService.UpdateAsync(id, item, userEmail);
                
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
                var success = await _areaEmpresaService.DeleteAsync(id, userEmail);

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

