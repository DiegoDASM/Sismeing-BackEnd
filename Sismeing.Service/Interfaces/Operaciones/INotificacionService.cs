using Sismeing.Domain.Entities.Operaciones;

namespace Sismeing.Service.Interfaces.Operaciones
{
    public interface INotificacionService
    {
        Task<IEnumerable<Notificacion>> GetByUsuarioAsync(int usuarioId);
        Task<int> GetNoLeidasCountAsync(int usuarioId);
        Task<Notificacion> CreateAsync(Notificacion item, string usuarioRegistro);

        /// <summary>
        /// Avisa a los supervisores (in-app y por correo) de que un informe
        /// quedo esperando aprobacion. No lanza excepciones.
        /// </summary>
        Task NotificarPendienteAprobacionAsync(
            string tipoServicio, string origen, int referenciaId,
            string numeroInforme, string tecnico, string cliente, string usuarioRegistro);
        Task<bool> MarcarLeidaAsync(int id, string usuarioModificacion);
        Task<int> MarcarTodasLeidasAsync(int usuarioId, string usuarioModificacion);
        Task<bool> DeleteAsync(int id, string usuarioEliminacion);
    }
}
