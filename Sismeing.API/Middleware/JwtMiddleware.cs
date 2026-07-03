using Microsoft.AspNetCore.Http;

namespace Sismeing.API.Middleware
{
    /// <summary>
    /// Adjunta los claims del usuario autenticado al HttpContext.Items para uso
    /// en los controllers. El token YA fue validado por UseAuthentication(),
    /// aquí solo se leen los claims (sin re-validar).
    /// </summary>
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;

        public JwtMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.User.Identity?.IsAuthenticated == true)
            {
                context.Items["UserId"]    = context.User.FindFirst("id")?.Value;
                context.Items["UserEmail"] = context.User.FindFirst("email")?.Value;
                context.Items["UserRole"]  = context.User.FindFirst("rol")?.Value;
                context.Items["EmpresaId"] = context.User.FindFirst("empresa_id")?.Value;
            }

            await _next(context);
        }
    }
}
