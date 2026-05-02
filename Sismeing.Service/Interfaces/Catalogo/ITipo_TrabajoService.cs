using Sismeing.Domain.Entities.Catalogo;

namespace Sismeing.Service.Interfaces.Catalogo
{
    public interface ITipo_TrabajoService
    {
        Task<IEnumerable<Tipo_Trabajo>> GetAllAsync();
        Task<Tipo_Trabajo?> GetByIdAsync(int id);
        Task<Tipo_Trabajo> CreateAsync(Tipo_Trabajo item, string usuarioRegistro);
        Task<bool> UpdateAsync(int id, Tipo_Trabajo item, string usuarioModificacion);
        Task<bool> DeleteAsync(int id, string usuarioEliminacion);
    }
}
