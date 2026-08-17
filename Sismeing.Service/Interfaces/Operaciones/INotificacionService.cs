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
        /// <summary>
        /// Avisa in-app que un servicio recien creado espera revision: al
        /// supervisor asignado y a los administradores. No lanza excepciones.
        /// </summary>
        Task NotificarNuevoServicioAsync(
            string tipoServicio, string origen, int referenciaId,
            string numeroInforme, int? supervisorId, string usuarioRegistro);

        /// <summary>
        /// Avisa a los usuarios Cliente de la empresa que el informe tiene la
        /// aprobacion interna y espera la suya. No lanza excepciones.
        /// </summary>
        Task NotificarAprobacionClienteAsync(
            string tipoServicio, string origen, int referenciaId,
            string numeroInforme, int? empresaId, string usuarioRegistro);

        Task<bool> MarcarLeidaAsync(int id, string usuarioModificacion);
        Task<int> MarcarTodasLeidasAsync(int usuarioId, string usuarioModificacion);
        Task<bool> DeleteAsync(int id, string usuarioEliminacion);
    }
}
