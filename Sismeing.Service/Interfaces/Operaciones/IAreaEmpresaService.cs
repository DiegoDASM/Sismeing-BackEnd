using Sismeing.Domain.Entities.Operaciones;

namespace Sismeing.Service.Interfaces.Operaciones
{
    public interface IArea_EmpresaService
    {
        Task<IEnumerable<Area_Empresa>> GetAllAsync();
        Task<Area_Empresa?> GetByIdAsync(int id);
        Task<Area_Empresa> CreateAsync(Area_Empresa item, string usuarioRegistro);
        Task<bool> UpdateAsync(int id, Area_Empresa item, string usuarioModificacion);
        Task<bool> DeleteAsync(int id, string usuarioEliminacion);
    }
}
