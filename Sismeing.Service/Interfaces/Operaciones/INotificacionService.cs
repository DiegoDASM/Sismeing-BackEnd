using Sismeing.Domain.Entities.Operaciones;

namespace Sismeing.Service.Interfaces.Operaciones
{
    public interface INotificacionService
    {
        Task<IEnumerable<Notificacion>> GetByUsuarioAsync(int usuarioId);
        Task<int> GetNoLeidasCountAsync(int usuarioId);
        Task<Notificacion> CreateAsync(Notificacion item, string usuarioRegistro);
        Task<bool> MarcarLeidaAsync(int id, string usuarioModificacion);
        Task<int> MarcarTodasLeidasAsync(int usuarioId, string usuarioModificacion);
        Task<bool> DeleteAsync(int id, string usuarioEliminacion);
    }
}
