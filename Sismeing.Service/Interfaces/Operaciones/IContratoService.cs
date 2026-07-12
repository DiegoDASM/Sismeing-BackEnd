using Sismeing.Domain.Entities.Operaciones;

namespace Sismeing.Service.Interfaces.Operaciones
{
    public interface IContratoService
    {
        Task<IEnumerable<Contrato>> GetAllAsync();
        Task<Contrato?> GetByIdAsync(int id);
        Task<Contrato> CreateAsync(Contrato item, string usuarioRegistro);
        Task<bool> UpdateAsync(int id, Contrato item, string usuarioModificacion);
        Task<bool> DeleteAsync(int id, string usuarioEliminacion);
        Task<bool> ReactivarAsync(int id, string usuario);

    }
}
