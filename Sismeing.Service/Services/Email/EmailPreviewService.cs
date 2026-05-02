using RazorLight;
using Sismeing.Service.Interfaces.Comunes;

namespace Sismeing.Service.Services.Email
{
    public class EmailPreviewService : IEmailPreviewService
    {
        private readonly IRazorLightEngine _engine;

        public EmailPreviewService()
        {
            _engine = new RazorLightEngineBuilder()
                .UseEmbeddedResourcesProject(typeof(Sismeing.Domain.Models.Emails.WelcomeModel).Assembly)
                .UseMemoryCachingProvider()
                .Build();
        }

        public async Task<string> RenderTemplateAsync<T>(string templateName, T model)
        {
            // La ruta base para los recursos embebidos dependerá de la estructura de carpetas.
            // Usualmente es "Plantillas.NombreArchivo.cshtml"
            string viewPath = $"Plantillas.{templateName}.cshtml";
            return await _engine.CompileRenderAsync(viewPath, model);
        }
    }
}
