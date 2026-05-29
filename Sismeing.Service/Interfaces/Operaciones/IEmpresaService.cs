using Sismeing.Domain.Entities.Operaciones;

namespace Sismeing.Service.Interfaces.Operaciones
{
    public interface IEmpresaService
    {
        Task<IEnumerable<Empresa>> GetAllAsync();
        Task<Empresa?> GetByIdAsync(int id);
        Task<Empresa> CreateAsync(Empresa item, string usuarioRegistro);
        Task<bool> UpdateAsync(int id, Empresa item, string usuarioModificacion);
        Task<bool> DeleteAsync(int id, string usuarioEliminacion);
        Task<string?> UpdateLogoAsync(int id, string logoPath);
    }
}
