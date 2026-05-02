using Sismeing.Domain.Entities.Catalogo;

namespace Sismeing.Service.Interfaces.Catalogo
{
    public interface ITrabajoService
    {
        Task<IEnumerable<Trabajo>> GetAllAsync();
        Task<Trabajo?> GetByIdAsync(int id);
        Task<Trabajo> CreateAsync(Trabajo item, string usuarioRegistro);
        Task<bool> UpdateAsync(int id, Trabajo item, string usuarioModificacion);
        Task<bool> DeleteAsync(int id, string usuarioEliminacion);
    }
}
