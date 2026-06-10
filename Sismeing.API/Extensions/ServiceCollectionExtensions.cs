using Microsoft.AspNetCore.Identity;
using Sismeing.Domain.Entities.Operaciones;
using Sismeing.Service.Interfaces.Catalogo;
using Sismeing.Service.Interfaces.Comunes;
using Sismeing.Service.Interfaces.Operaciones;
using Sismeing.Service.Services.Catalogo;
using Sismeing.Service.Services.Comunes;
using Sismeing.Service.Services.Email;
using Sismeing.Service.Services.Operaciones;

namespace Sismeing.API.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // ── Comunes ──
            services.AddSingleton<ICloudinaryService, CloudinaryService>();
            services.AddScoped<IEmailPreviewService, EmailPreviewService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IAuditoriaService, AuditoriaService>();


            // ── Operaciones ──
            services.AddScoped<IInstalacionService, InstalacionService>();
            services.AddScoped<IUsuarioService, UsuarioService>();
            services.AddScoped<IPasswordHasher<Usuario>, PasswordHasher<Usuario>>();
            services.AddScoped<IEmpresaService, EmpresaService>();
            services.AddScoped<IDireccion_EmpresaService, DireccionEmpresaService>();
            services.AddScoped<IArea_EmpresaService, AreaEmpresaService>();
            services.AddScoped<IContratoService, ContratoService>();
            services.AddScoped<IEquipoService, EquipoService>();
            services.AddScoped<IFoto_InstalacionService, FotoInstalacionService>();
            services.AddScoped<IMantenimientoService, MantenimientoService>();
            services.AddScoped<IFoto_MantenimientoService, FotoMantenimientoService>();
            services.AddScoped<IVisita_TecnicaService, VisitaTecnicaService>();
            services.AddScoped<IMedicionService, MedicionService>();

            // ── Catalogo ──
            services.AddScoped<IRolService, RolService>();
            services.AddScoped<IEstadoService, EstadoService>();
            services.AddScoped<IMarcaService, MarcaService>();
            services.AddScoped<IModeloService, ModeloService>();
            services.AddScoped<ITipo_EquipoService, Tipo_EquipoService>();
            services.AddScoped<ITipo_MantenimientoService, Tipo_MantenimientoService>();
            services.AddScoped<ITipo_TrabajoService, Tipo_TrabajoService>();
            services.AddScoped<ITipo_InformeService, Tipo_InformeService>();
            services.AddScoped<ITrabajoService, TrabajoService>();

            return services;
        }
    }
}
