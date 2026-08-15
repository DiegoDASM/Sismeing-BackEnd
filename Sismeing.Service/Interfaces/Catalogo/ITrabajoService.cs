using Sismeing.Domain.Entities.Catalogo;

namespace Sismeing.Service.Interfaces.Catalogo
{
    public interface ITrabajoService
    {
        Task<IEnumerable<Trabajo>> GetAllAsync();
        Task<Trabajo?> GetByIdAsync(int id);
        Task<IEnumerable<Trabajo>> GetByMantenimientoAsync(int mantenimientoId);
        Task<IEnumerable<Trabajo>> GetByInstalacionAsync(int instalacionId);
        Task<Trabajo> CreateAsync(Trabajo item, string usuarioRegistro);
        Task<IEnumerable<Trabajo>> ReemplazarPorMantenimientoAsync(int mantenimientoId, IEnumerable<Trabajo> items, string usuarioRegistro);
        Task<IEnumerable<Trabajo>> ReemplazarPorInstalacionAsync(int instalacionId, IEnumerable<Trabajo> items, string usuarioRegistro);
        Task<bool> UpdateAsync(int id, Trabajo item, string usuarioModificacion);
        Task<bool> DeleteAsync(int id, string usuarioEliminacion);
    }
}
