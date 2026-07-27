using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sismeing.Domain.Entities.Catalogo;
using Sismeing.Service;
using Sismeing.Service.Interfaces.Catalogo;

namespace Sismeing.API.Controllers.Catalogo
{
    //[Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class Tipo_EquipoController : Controller
    {
        private readonly ITipo_EquipoService _tipo_EquipoService;

        public Tipo_EquipoController(ITipo_EquipoService tipo_equiposervice)
        {
            _tipo_EquipoService = tipo_equiposervice;
        }

        [HttpGet]
        public async Task<ActionResult> GetAll()
        {
            try
            {
                var data = await _tipo_EquipoService.GetAllAsync();
                return Ok(new JsonResponse<IEnumerable<Tipo_Equipo>>(data));
            }
            catch (Exception ex)
            {
                return BadRequest(new JsonResponse<IEnumerable<Tipo_Equipo>>(null, ex.Message, ResponseStatus.error));
            }
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult> GetById(int id)
        {
            try
            {
                var data = await _tipo_EquipoService.GetByIdAsync(id);
                if (data == null)
                    return NotFound(new JsonResponse<Tipo_Equipo>(null, "No encontrado", ResponseStatus.error));
                return Ok(new JsonResponse<Tipo_Equipo>(data));
            }
            catch (Exception ex)
            {
                return BadRequest(new JsonResponse<Tipo_Equipo>(null, ex.Message, ResponseStatus.error));
            }
        }

        [HttpPost]
        [Authorize(Roles = "Administrador,Supervisor,Tecnico,SuperAdmin")]
        public async Task<ActionResult> Create([FromBody] Tipo_Equipo item)
        {
            try
            {
                var userEmail = HttpContext.Items["UserEmail"]?.ToString() ?? "SYSTEM";
                var result = await _tipo_EquipoService.CreateAsync(item, userEmail);

                return Ok(new JsonResponse<Tipo_Equipo>(result));
            }
            catch (Exception ex)
            {
                return BadRequest(new JsonResponse<Tipo_Equipo>(null, ex.Message, ResponseStatus.error));
            }
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "Administrador,Supervisor,Tecnico,SuperAdmin")]
        public async Task<ActionResult> Update(int id, [FromBody] Tipo_Equipo item)
        {
            try
            {
                if (id != item.Id)
                    return BadRequest(new JsonResponse<bool>(false, "El ID no coincide", ResponseStatus.error));

                var userEmail = HttpContext.Items["UserEmail"]?.ToString() ?? "SYSTEM";
                var success = await _tipo_EquipoService.UpdateAsync(id, item, userEmail);
                
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
        [Authorize(Roles = "Administrador,Supervisor,Tecnico,SuperAdmin")]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                var userEmail = HttpContext.Items["UserEmail"]?.ToString() ?? "SYSTEM";
                var success = await _tipo_EquipoService.DeleteAsync(id, userEmail);

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
