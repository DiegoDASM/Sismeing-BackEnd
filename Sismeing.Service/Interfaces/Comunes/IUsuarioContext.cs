namespace Sismeing.Service.Interfaces.Comunes
{
    // Datos del usuario autenticado (leidos del token via HttpContext.Items),
    // para aplicar reglas por rol/empresa dentro de los servicios.
    public interface IUsuarioContext
    {
        string? Rol { get; }
        int? EmpresaId { get; }
        int? UsuarioId { get; }
        bool EsCliente { get; }
    }
}
