using Sismeing.Domain.Entities.Catalogo;

namespace Sismeing.Service.Interfaces.Catalogo
{
    public interface ITipo_MantenimientoService
    {
        Task<IEnumerable<Tipo_Mantenimiento>> GetAllAsync();
        Task<Tipo_Mantenimiento?> GetByIdAsync(int id);
        Task<Tipo_Mantenimiento> CreateAsync(Tipo_Mantenimiento item, string usuarioRegistro);
        Task<bool> UpdateAsync(int id, Tipo_Mantenimiento item, string usuarioModificacion);
        Task<bool> DeleteAsync(int id, string usuarioEliminacion);
    }
}
