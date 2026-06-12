using Sismeing.Domain.Entities.Operaciones;
using Sismeing.Domain.Enums;

namespace Sismeing.Service.Interfaces.Comunes
{
    public interface IEmailService
    {
        Task SendAsync<T>(string toEmail, string subject, string templateName, T model);
        Task<object> TestSmtpConnectionAsync();
        Task EnviarCorreoPredefinidoAsync(TipoCorreo tipo, Usuario usuario, string? tokenAdicional = null);
    }
}
