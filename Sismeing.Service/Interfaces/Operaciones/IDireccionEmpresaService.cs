using Sismeing.Domain.Entities.Operaciones;

namespace Sismeing.Service.Interfaces.Operaciones
{
    public interface IDireccion_EmpresaService
    {
        Task<IEnumerable<Direccion_Empresa>> GetAllAsync();
        Task<Direccion_Empresa?> GetByIdAsync(int id);
        Task<Direccion_Empresa> CreateAsync(Direccion_Empresa item, string usuarioRegistro);
        Task<bool> UpdateAsync(int id, Direccion_Empresa item, string usuarioModificacion);
        Task<bool> DeleteAsync(int id, string usuarioEliminacion);
    }
}
