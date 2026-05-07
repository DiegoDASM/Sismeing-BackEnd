using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;
using Sismeing.Service.Interfaces.Comunes;

namespace Sismeing.Service.Services.Email
{
    public class EmailService : IEmailService
    {
        private readonly IEmailPreviewService _previewService;
        private readonly IConfiguration _config;

        public EmailService(IEmailPreviewService previewService, IConfiguration config)
        {
            _previewService = previewService;
            _config = config;
        }

        public async Task SendAsync<T>(string toEmail, string subject, string templateName, T model)
        {
            var htmlBody = await _previewService.RenderTemplateAsync(templateName, model);

            var message = new MimeMessage();
            var senderName = _config["EmailSettings:SenderName"] ?? "Sismeing";
            var senderEmail = _config["EmailSettings:SmtpUser"];
            
            message.From.Add(new MailboxAddress(senderName, senderEmail));
            message.To.Add(new MailboxAddress("", toEmail));
            message.Subject = subject;

            var bodyBuilder = new BodyBuilder { HtmlBody = htmlBody };
            message.Body = bodyBuilder.ToMessageBody();

            using var client = new SmtpClient();
            var host = _config["EmailSettings:SmtpHost"];
            var port = int.Parse(_config["EmailSettings:SmtpPort"] ?? "587");
            var pass = _config["EmailSettings:SmtpPass"];

            // Configurar validación de certificados SSL/TLS
            client.ServerCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => 
            {
                if (sslPolicyErrors == System.Net.Security.SslPolicyErrors.None) return true;
                if (host != null && (host.Contains("office365.com") || host.Contains("outlook.com"))) return true;
                return true; // Permitir por defecto en desarrollo local
            };
            client.CheckCertificateRevocation = false;

            // Determinar política de seguridad según proveedor
            SecureSocketOptions secureSocketOptions = SecureSocketOptions.Auto;
            if (host != null)
            {
                if (host.Contains("office365.com") || host.Contains("outlook.com"))
                    secureSocketOptions = SecureSocketOptions.StartTls;
                else if (host.Contains("gmail.com"))
                    secureSocketOptions = port == 465 ? SecureSocketOptions.SslOnConnect : SecureSocketOptions.StartTls;
                else
                    secureSocketOptions = port switch { 465 => SecureSocketOptions.SslOnConnect, 587 => SecureSocketOptions.StartTls, 25 => SecureSocketOptions.StartTls, _ => SecureSocketOptions.Auto };
            }

            bool connected = false;
            Exception? lastException = null;

            // Arreglo de configuraciones de respaldo (Fallbacks)
            var connectConfigs = new[]
            {
                new { Host = host, Port = port, Options = secureSocketOptions },
                new { Host = host, Port = port, Options = SecureSocketOptions.Auto },
                new { Host = "smtp-mail.outlook.com", Port = 587, Options = SecureSocketOptions.StartTls },
                new { Host = "smtp.office365.com", Port = 587, Options = SecureSocketOptions.StartTls },
                new { Host = "smtp.gmail.com", Port = 587, Options = SecureSocketOptions.StartTls }
            };

            foreach (var config in connectConfigs)
            {
                try
                {
                    if (string.IsNullOrEmpty(config.Host)) continue;
                    await client.ConnectAsync(config.Host, config.Port, config.Options);
                    connected = true;
                    break;
                }
                catch (Exception ex)
                {
                    lastException = ex;
                    if (client.IsConnected) await client.DisconnectAsync(false);
                }
            }

            if (!connected)
                throw new InvalidOperationException($"No se pudo establecer conexión SMTP después de múltiples intentos: {lastException?.Message}");

            await client.AuthenticateAsync(senderEmail, pass);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }

        public async Task<object> TestSmtpConnectionAsync()
        {
            var results = new List<string>();
            var host = _config["EmailSettings:SmtpHost"] ?? "smtp.office365.com";
            var user = _config["EmailSettings:SmtpUser"];
            var pass = _config["EmailSettings:SmtpPass"];

            var configurations = new[]
            {
                new { Host = host, Port = 587, Options = SecureSocketOptions.StartTls },
                new { Host = host, Port = 587, Options = SecureSocketOptions.Auto },
                new { Host = "smtp-mail.outlook.com", Port = 587, Options = SecureSocketOptions.StartTls }
            };

            foreach (var config in configurations)
            {
                try
                {
                    using var client = new SmtpClient();
                    client.ServerCertificateValidationCallback = (s, c, h, e) => true;
                    client.CheckCertificateRevocation = false;

                    await client.ConnectAsync(config.Host, config.Port, config.Options);
                    results.Add($"Conexion exitosa: {config.Host}:{config.Port}");

                    try
                    {
                        await client.AuthenticateAsync(user, pass);
                        results.Add($"Autenticacion exitosa");
                        await client.DisconnectAsync(true);
                        return new { Success = true, WorkingConfig = config, Results = results };
                    }
                    catch (Exception authEx)
                    {
                        results.Add($"Error de autenticacion: {authEx.Message}");
                    }
                    await client.DisconnectAsync(true);
                }
                catch (Exception ex)
                {
                    results.Add($"Error de conexion: {ex.Message}");
                }
            }

            return new { Success = false, Results = results };
        }
    }
}
