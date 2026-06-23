using System.Threading.Tasks;

namespace Sismeing.Service.Interfaces.Operaciones
{
    public interface IReporteService
    {
        Task<string> InstalacionDatosAsync(int id);
        Task<string> InstalacionFotograficoAsync(int id);
        Task<string> MantenimientoDatosAsync(int id);
        Task<string> MantenimientoFotograficoAsync(int id);
        Task<string> VisitaDatosAsync(int id);
        Task<string> VisitaFotograficoAsync(int id);
    }
}
