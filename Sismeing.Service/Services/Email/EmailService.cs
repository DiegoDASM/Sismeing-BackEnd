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

            client.ServerCertificateValidationCallback = (s, c, h, e) => true;

            await client.ConnectAsync(host, port, SecureSocketOptions.Auto);
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
