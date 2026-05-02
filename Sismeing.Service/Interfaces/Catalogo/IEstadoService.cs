using Sismeing.Domain.Entities.Catalogo;

namespace Sismeing.Service.Interfaces.Catalogo
{
    public interface IEstadoService
    {
        Task<IEnumerable<Estado>> GetAllAsync();
        Task<Estado?> GetByIdAsync(int id);
        Task<Estado> CreateAsync(Estado item, string usuarioRegistro);
        Task<bool> UpdateAsync(int id, Estado item, string usuarioModificacion);
        Task<bool> DeleteAsync(int id, string usuarioEliminacion);
    }
}
