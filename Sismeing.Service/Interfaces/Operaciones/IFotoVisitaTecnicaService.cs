using Sismeing.Domain.Entities.Operaciones;

namespace Sismeing.Service.Interfaces.Operaciones
{
    public interface IFoto_VisitaTecnicaService
    {
        Task<IEnumerable<Foto_VisitaTecnica>> GetAllAsync();
        Task<Foto_VisitaTecnica?> GetByIdAsync(int id);
        Task<IEnumerable<Foto_VisitaTecnica>> GetByVisitaTecnicaIdAsync(int visitaTecnicaId);
        Task<Foto_VisitaTecnica> CreateAsync(Foto_VisitaTecnica item, string usuarioRegistro);
        Task<bool> UpdateAsync(int id, Foto_VisitaTecnica item, string usuarioModificacion);
        Task<bool> DeleteAsync(int id, string usuarioEliminacion);
    }
}
