namespace Sismeing.Service.Interfaces.Operaciones
{
    /// <summary>
    /// Genera el HTML de los informes (datos y fotográfico) a partir de los registros.
    /// Devuelve null cuando el registro no existe.
    /// </summary>
    public interface IReporteService
    {
        Task<string?> InstalacionDatosAsync(int id);
        Task<string?> InstalacionFotograficoAsync(int id);

        Task<string?> MantenimientoDatosAsync(int id);
        Task<string?> MantenimientoFotograficoAsync(int id);

        Task<string?> VisitaDatosAsync(int id);
        Task<string?> VisitaFotograficoAsync(int id);

        /// <summary>Hoja de vida completa del equipo (destino del código QR).</summary>
        Task<string?> EquipoHojaVidaAsync(int id);
    }
}
