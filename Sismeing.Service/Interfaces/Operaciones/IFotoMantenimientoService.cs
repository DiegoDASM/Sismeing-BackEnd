using Sismeing.Domain.Entities.Operaciones;

namespace Sismeing.Service.Interfaces.Operaciones
{
    public interface IFoto_MantenimientoService
    {
        Task<IEnumerable<Foto_Mantenimiento>> GetAllAsync();
        Task<Foto_Mantenimiento?> GetByIdAsync(int id);
        Task<IEnumerable<Foto_Mantenimiento>> GetByMantenimientoIdAsync(int mantenimientoId);
        Task<Foto_Mantenimiento> CreateAsync(Foto_Mantenimiento item, string usuarioRegistro);
        Task<bool> UpdateAsync(int id, Foto_Mantenimiento item, string usuarioModificacion);
        Task<bool> DeleteAsync(int id, string usuarioEliminacion);
    }
}
