using Microsoft.AspNetCore.Mvc;
using Sismeing.Service;
using Sismeing.Service.Interfaces.Operaciones;

namespace Sismeing.API.Controllers.Operaciones
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReporteController : Controller
    {
        private readonly IReporteService _reporteService;

        public ReporteController(IReporteService reporteService)
        {
            _reporteService = reporteService;
        }

        private ActionResult Html(string? html)
        {
            if (html == null)
                return NotFound(new JsonResponse<string>(null, "Registro no encontrado", ResponseStatus.error));
            return Content(html, "text/html; charset=utf-8");
        }

        // ── Instalación ──
        [HttpGet("instalacion/{id:int}")]
        public async Task<ActionResult> InstalacionDatos(int id) => Html(await _reporteService.InstalacionDatosAsync(id));

        [HttpGet("instalacion/{id:int}/fotografico")]
        public async Task<ActionResult> InstalacionFotografico(int id) => Html(await _reporteService.InstalacionFotograficoAsync(id));

        // ── Mantenimiento ──
        [HttpGet("mantenimiento/{id:int}")]
        public async Task<ActionResult> MantenimientoDatos(int id) => Html(await _reporteService.MantenimientoDatosAsync(id));

        [HttpGet("mantenimiento/{id:int}/fotografico")]
        public async Task<ActionResult> MantenimientoFotografico(int id) => Html(await _reporteService.MantenimientoFotograficoAsync(id));

        // ── Visita Técnica ──
        [HttpGet("visita/{id:int}")]
        public async Task<ActionResult> VisitaDatos(int id) => Html(await _reporteService.VisitaDatosAsync(id));

        [HttpGet("visita/{id:int}/fotografico")]
        public async Task<ActionResult> VisitaFotografico(int id) => Html(await _reporteService.VisitaFotograficoAsync(id));
    }
}
