using System.Threading.Tasks;
using Sismeing.Service.Interfaces.Operaciones;

namespace Sismeing.Service.Services.Operaciones
{
    public class ReporteService : IReporteService
    {
        public Task<string> InstalacionDatosAsync(int id)
        {
            return Task.FromResult("<h1>Datos de Instalación</h1><p>En desarrollo...</p>");
        }

        public Task<string> InstalacionFotograficoAsync(int id)
        {
            return Task.FromResult("<h1>Reporte Fotográfico de Instalación</h1><p>En desarrollo...</p>");
        }

        public Task<string> MantenimientoDatosAsync(int id)
        {
            return Task.FromResult("<h1>Datos de Mantenimiento</h1><p>En desarrollo...</p>");
        }

        public Task<string> MantenimientoFotograficoAsync(int id)
        {
            return Task.FromResult("<h1>Reporte Fotográfico de Mantenimiento</h1><p>En desarrollo...</p>");
        }

        public Task<string> VisitaDatosAsync(int id)
        {
            return Task.FromResult("<h1>Datos de Visita Técnica</h1><p>En desarrollo...</p>");
        }

        public Task<string> VisitaFotograficoAsync(int id)
        {
            return Task.FromResult("<h1>Reporte Fotográfico de Visita Técnica</h1><p>En desarrollo...</p>");
        }
    }
}
