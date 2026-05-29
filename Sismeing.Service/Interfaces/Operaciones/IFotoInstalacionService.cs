using Sismeing.Domain.Entities.Operaciones;

namespace Sismeing.Service.Interfaces.Operaciones
{
    public interface IFoto_InstalacionService
    {
        Task<IEnumerable<Foto_Instalacion>> GetAllAsync();
        Task<Foto_Instalacion?> GetByIdAsync(int id);
        Task<IEnumerable<Foto_Instalacion>> GetByInstalacionIdAsync(int instalacionId);
        Task<Foto_Instalacion> CreateAsync(Foto_Instalacion item, string usuarioRegistro);
        Task<bool> UpdateAsync(int id, Foto_Instalacion item, string usuarioModificacion);
        Task<bool> DeleteAsync(int id, string usuarioEliminacion);
    }
}
