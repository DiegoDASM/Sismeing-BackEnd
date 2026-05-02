using Sismeing.Domain.Entities.Operaciones;

namespace Sismeing.Service.Interfaces.Operaciones
{
    public interface IMedicionService
    {
        Task<IEnumerable<Medicion>> GetAllAsync();
        Task<Medicion?> GetByIdAsync(int id);
        Task<Medicion> CreateAsync(Medicion item, string usuarioRegistro);
        Task<bool> UpdateAsync(int id, Medicion item, string usuarioModificacion);
        Task<bool> DeleteAsync(int id, string usuarioEliminacion);
    }
}
