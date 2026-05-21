using Microsoft.AspNetCore.Mvc;
using Sismeing.Domain.Models.Emails;
using Sismeing.Service.Interfaces.Comunes;

namespace Sismeing.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmailPreviewController : ControllerBase
    {
        private readonly IEmailPreviewService _emailPreviewService;
        private readonly IEmailService _emailService;

        public EmailPreviewController(IEmailPreviewService emailPreviewService, IEmailService emailService)
        {
            _emailPreviewService = emailPreviewService;
            _emailService = emailService;
        }

        [HttpGet("test-real-email")]
        public async Task<IActionResult> TestRealEmail([FromQuery] string email)
        {
            try
            {
                if (string.IsNullOrEmpty(email))
                    return BadRequest("Debes proporcionar un email en la query string ?email=tucorreo@ejemplo.com");

                var model = new MaintenanceReminderModel
                {
                    EmpresaCliente = "Banco Bolivariano",
                    TipoSistema = "Aire Acondicionado Central",
                    Equipos = new List<EquipoInfo>
                                {
                                    new EquipoInfo { Nombre = "Compresor Unidad A", ProximaFecha = DateTime.Now.AddDays(5) },
                                    new EquipoInfo { Nombre = "Chiller Principal", ProximaFecha = DateTime.Now.AddDays(7) }
                                }
                };

                await _emailService.SendAsync(email, "Recordatorio de Mantenimiento", "RecordatorioMantenimiento", model);
                return Ok($"Email enviado correctamente a {email}");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error enviando email: {ex.Message}");
            }
        }

        [HttpGet("test-smtp-connection")]
        public async Task<IActionResult> TestSmtpConnection()
        {
            try
            {
                var result = await _emailService.TestSmtpConnectionAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
