namespace Sismeing.Service.Interfaces.Comunes
{
    public interface IEmailPreviewService
    {
        Task<string> RenderTemplateAsync<T>(string templateName, T model);
    }
}
