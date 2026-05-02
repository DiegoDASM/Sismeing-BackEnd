using Sismeing.Domain.Entities.Catalogo;

namespace Sismeing.Service.Interfaces.Catalogo
{
    public interface ITipo_EquipoService
    {
        Task<IEnumerable<Tipo_Equipo>> GetAllAsync();
        Task<Tipo_Equipo?> GetByIdAsync(int id);
        Task<Tipo_Equipo> CreateAsync(Tipo_Equipo item, string usuarioRegistro);
        Task<bool> UpdateAsync(int id, Tipo_Equipo item, string usuarioModificacion);
        Task<bool> DeleteAsync(int id, string usuarioEliminacion);
    }
}
