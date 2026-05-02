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

        [HttpGet("preview/bienvenida")]
        public async Task<IActionResult> PreviewBienvenida()
        {
            try
            {
                var model = new WelcomeModel
                {
                    UserName = "Usuario de Prueba",
                    SetPasswordUrl = "https://sismeing.com/set-password?token=12345",
                    Email = "prueba@sismeing.com"
                };

                var html = await _emailPreviewService.RenderTemplateAsync("Bienvenida", model);
                return Content(html, "text/html");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error renderizando plantilla: {ex.Message}");
            }
        }

        [HttpGet("test-real-email")]
        public async Task<IActionResult> TestRealEmail([FromQuery] string email)
        {
            try
            {
                if (string.IsNullOrEmpty(email))
                    return BadRequest("Debes proporcionar un email en la query string ?email=tucorreo@ejemplo.com");

                var model = new WelcomeModel
                {
                    UserName = "Usuario de Prueba Real",
                    SetPasswordUrl = "https://sismeing.com/set-password?token=12345",
                    Email = email
                };

                await _emailService.SendAsync(email, "Bienvenido a Sismeing", "Bienvenida", model);
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
