namespace Sismeing.Domain.Entities.DTOs
{
    public class LoginResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public DateTime Expiracion { get; set; }
        public UsuarioDto Usuario { get; set; } = null!;
    }

    public class UsuarioDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public string CorreoElectronico { get; set; } = string.Empty;
        public string Cedula { get; set; } = string.Empty;
        public string? Telefono { get; set; }
        public string NombreRol { get; set; } = string.Empty;
        public int RolId { get; set; }
        public int EmpresaId { get; set; }
        public string NombreEmpresa { get; set; } = string.Empty;
        public bool Verificado { get; set; }
        public bool Activo { get; set; } = true;
    }

    public class RegisterRequestDto
    {
        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public string Cedula { get; set; } = string.Empty;
        public string CorreoElectronico { get; set; } = string.Empty;
        public string? Telefono { get; set; }
        public string Password { get; set; } = string.Empty;
        public int EmpresaId { get; set; }
        public int RolId { get; set; }
    }
}
