using Microsoft.Extensions.DependencyInjection;

namespace Sismeing.Service
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSismeingService(this IServiceCollection services)
        {
            // Aquí puedes agregar tus servicios personalizados
            // Por ejemplo:
            // services.AddScoped<IMiServicio, MiServicio>();


            return services;
        }
    }
}
