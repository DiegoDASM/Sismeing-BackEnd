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
                .UseEmbeddedResourcesProject(typeof(Sismeing.Domain.Models.Emails.WelcomeModel).Assembly, "Sismeing.Domain.Plantillas")
                .UseMemoryCachingProvider()
                .Build();
        }

        public async Task<string> RenderTemplateAsync<T>(string templateName, T model)
        {
            // Usar directamente el nombre porque el namespace ya está en el constructor
            string viewPath = $"{templateName}.cshtml";
            return await _engine.CompileRenderAsync(viewPath, model);
        }
    }
}
