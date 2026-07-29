using Sismeing.Domain.Entities.Operaciones;

namespace Sismeing.Service.Interfaces.Operaciones
{
    public interface IMedicionService
    {
        Task<IEnumerable<Medicion>> GetAllAsync();
        Task<Medicion?> GetByIdAsync(int id);
        Task<IEnumerable<Medicion>> GetByInformeAsync(int informeId, int equipoId, string? origen = null);
        Task<Medicion> CreateAsync(Medicion item, string usuarioRegistro);
        Task<IEnumerable<Medicion>> CreateBatchAsync(IEnumerable<Medicion> items, string usuarioRegistro);
        Task<IEnumerable<Medicion>> ReemplazarPorInformeAsync(int informeId, int equipoId, IEnumerable<Medicion> items, string usuarioRegistro, string? origen = null);
        Task<bool> UpdateAsync(int id, Medicion item, string usuarioModificacion);
        Task<bool> DeleteAsync(int id, string usuarioEliminacion);
    }
}
