using Sismeing.Domain.Entities.Operaciones;

namespace Sismeing.Service.Interfaces.Operaciones
{
    public interface IEquipoService
    {
        Task<IEnumerable<Equipo>> GetAllAsync();
        Task<Equipo?> GetByIdAsync(int id);
        Task<Equipo> CreateAsync(Equipo item, string usuarioRegistro);
        Task<bool> UpdateAsync(int id, Equipo item, string usuarioModificacion);
        Task<bool> DeleteAsync(int id, string usuarioEliminacion);
    }
}
