using Sismeing.Domain.Entities.Operaciones;

namespace Sismeing.Service.Interfaces.Operaciones
{
    public interface IUsuarioService
    {
        Task<IEnumerable<Usuario>> GetAllAsync();
        Task<IEnumerable<Usuario>> GetTodosAsync();
        Task<bool> ReactivarAsync(int id, string usuario);
        Task<Usuario?> GetByIdAsync(int id);
        Task<Usuario> CreateAsync(Usuario item, string usuarioRegistro);
        Task<bool> UpdateAsync(int id, Usuario item, string usuarioModificacion);
        Task<bool> DeleteAsync(int id, string usuarioEliminacion);
        Task<bool> UpdatePerfilAsync(int id, string nombre, string apellido, string? telefono, string userEmail);

        // ── Invitación de usuario ──
        // rolId opcional: si no se envía se asume el rol "Cliente" (invitación
        // de encargado desde la ficha del cliente).
        Task InvitarUsuarioAsync(string correo, int? rolId, int empresaId, string usuarioRegistro);
        Task<(string correo, string empresaNombre)?> GetInvitacionAsync(string token);
        Task<bool> CompletarRegistroAsync(string token, string nombre, string apellido, string cedula, string contrasena);
    }
}
