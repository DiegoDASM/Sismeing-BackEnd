namespace Sismeing.Service.Interfaces.Comunes
{
    public interface IEmailService
    {
        Task SendAsync<T>(string toEmail, string subject, string templateName, T model);
        Task<object> TestSmtpConnectionAsync();
    }
}
