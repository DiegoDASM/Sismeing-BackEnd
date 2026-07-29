using Microsoft.AspNetCore.Http;
using Sismeing.Service.Interfaces.Comunes;

namespace Sismeing.Service.Services.Comunes
{
    // Lee el rol, la empresa y el id del usuario que JwtMiddleware dejo en
    // HttpContext.Items tras validar el token.
    public class UsuarioContext : IUsuarioContext
    {
        private readonly IHttpContextAccessor _accessor;

        public UsuarioContext(IHttpContextAccessor accessor)
        {
            _accessor = accessor;
        }

        private object? Item(string clave) => _accessor.HttpContext?.Items[clave];

        public string? Rol => Item("UserRole")?.ToString();

        public int? EmpresaId =>
            int.TryParse(Item("EmpresaId")?.ToString(), out var id) ? id : null;

        public int? UsuarioId =>
            int.TryParse(Item("UserId")?.ToString(), out var id) ? id : null;

        public bool EsCliente => string.Equals(Rol, "Cliente", System.StringComparison.OrdinalIgnoreCase);
    }
}
