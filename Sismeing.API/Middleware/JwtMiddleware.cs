using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace Sismeing.API.Middleware
{
    /// <summary>
    /// Middleware que valida el JWT en cada request y adjunta los claims al contexto HTTP.
    /// </summary>
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _configuration;

        public JwtMiddleware(RequestDelegate next, IConfiguration configuration)
        {
            _next = next;
            _configuration = configuration;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var token = context.Request.Headers["Authorization"]
                .FirstOrDefault()?.Split(" ").Last();

            if (token != null)
                AttachUserToContext(context, token);

            await _next(context);
        }

        private void AttachUserToContext(HttpContext context, string token)
        {
            try
            {
                var jwtSettings = _configuration.GetSection("JwtSettings");
                var secretKey = jwtSettings["SecretKey"]!;
                var key = Encoding.UTF8.GetBytes(secretKey);

                var tokenHandler = new JwtSecurityTokenHandler();
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = jwtSettings["Issuer"],
                    ValidateAudience = true,
                    ValidAudience = jwtSettings["Audience"],
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;

                // Adjuntar claims al contexto para uso en controllers
                context.Items["UserId"] = int.Parse(jwtToken.Claims.First(c => c.Type == "id").Value);
                context.Items["UserEmail"] = jwtToken.Claims.First(c => c.Type == "email").Value;
                context.Items["UserRole"] = jwtToken.Claims.First(c => c.Type == "rol").Value;
                context.Items["EmpresaId"] = int.Parse(jwtToken.Claims.First(c => c.Type == "empresa_id").Value);
            }
            catch
            {
                // Si el token es inválido, no se adjunta nada.
                // El atributo [Authorize] retornará 401 automáticamente.
            }
        }
    }
}
