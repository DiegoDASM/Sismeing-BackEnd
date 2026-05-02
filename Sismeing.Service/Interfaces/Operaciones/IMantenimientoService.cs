using Sismeing.Domain.Entities.Operaciones;

namespace Sismeing.Service.Interfaces.Operaciones
{
    public interface IMantenimientoService
    {
        Task<IEnumerable<Mantenimiento>> GetAllAsync();
        Task<Mantenimiento?> GetByIdAsync(int id);
        Task<Mantenimiento> CreateAsync(Mantenimiento item, string usuarioRegistro);
        Task<bool> UpdateAsync(int id, Mantenimiento item, string usuarioModificacion);
        Task<bool> DeleteAsync(int id, string usuarioEliminacion);
    }
}
