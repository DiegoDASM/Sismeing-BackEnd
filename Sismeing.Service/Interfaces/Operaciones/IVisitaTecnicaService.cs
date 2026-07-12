using Sismeing.Domain.Entities.Operaciones;

namespace Sismeing.Service.Interfaces.Operaciones
{
    public interface IVisita_TecnicaService
    {
        Task<IEnumerable<Visita_Tecnica>> GetAllAsync();
        Task<Visita_Tecnica?> GetByIdAsync(int id);
        Task<Visita_Tecnica> CreateAsync(Visita_Tecnica item, string usuarioRegistro);
        Task<bool> UpdateAsync(int id, Visita_Tecnica item, string usuarioModificacion);
        Task<bool> DeleteAsync(int id, string usuarioEliminacion);
        Task<bool> ReactivarAsync(int id, string usuario);

    }
}
