using Sismeing.Domain.Entities.Catalogo;

namespace Sismeing.Service.Interfaces.Catalogo
{
    public interface IModeloService
    {
        Task<IEnumerable<Modelo>> GetAllAsync();
        Task<Modelo?> GetByIdAsync(int id);
        Task<Modelo> CreateAsync(Modelo item, string usuarioRegistro);
        Task<bool> UpdateAsync(int id, Modelo item, string usuarioModificacion);
        Task<bool> DeleteAsync(int id, string usuarioEliminacion);
    }
}
