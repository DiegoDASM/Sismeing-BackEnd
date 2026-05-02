using Sismeing.Domain.Entities.Catalogo;

namespace Sismeing.Service.Interfaces.Catalogo
{
    public interface ITipo_InformeService
    {
        Task<IEnumerable<Tipo_Informe>> GetAllAsync();
        Task<Tipo_Informe?> GetByIdAsync(int id);
        Task<Tipo_Informe> CreateAsync(Tipo_Informe item, string usuarioRegistro);
        Task<bool> UpdateAsync(int id, Tipo_Informe item, string usuarioModificacion);
        Task<bool> DeleteAsync(int id, string usuarioEliminacion);
    }
}
