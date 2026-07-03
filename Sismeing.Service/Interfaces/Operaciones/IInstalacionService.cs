using Sismeing.Domain.Entities.Operaciones;

namespace Sismeing.Service.Interfaces.Operaciones
{
    public interface IInstalacionService
    {
        Task<IEnumerable<Instalacion>> GetAllAsync();
        Task<Instalacion?> GetByIdAsync(int id);
        Task<Instalacion> CreateAsync(Instalacion item, string usuarioRegistro);
        Task<bool> UpdateAsync(int id, Instalacion item, string usuarioModificacion);
        Task<bool> AprobarAsync(int id, string usuario);
        Task<bool> DeleteAsync(int id, string usuarioEliminacion);
    }
}
