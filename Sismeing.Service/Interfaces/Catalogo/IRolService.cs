using Sismeing.Domain.Entities.Catalogo;

namespace Sismeing.Service.Interfaces.Catalogo
{
    public interface IRolService
    {
        Task<IEnumerable<Rol>> GetAllAsync();
        Task<Rol?> GetByIdAsync(int id);
        Task<Rol> CreateAsync(Rol item, string usuarioRegistro);
        Task<bool> UpdateAsync(int id, Rol item, string usuarioModificacion);
        Task<bool> DeleteAsync(int id, string usuarioEliminacion);
    }
}
