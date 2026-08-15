namespace Sismeing.Domain.Models.Emails
{
    /// <summary>
    /// Correo que reciben los supervisores cuando un informe queda a la espera
    /// de su aprobacion. Aprobar es la unica atribucion que separa al rol
    /// Supervisor del rol Tecnico, por eso se les avisa expresamente.
    /// </summary>
    public class AprobacionPendienteModel
    {
        /// <summary>Nombre del supervisor destinatario.</summary>
        public string Supervisor { get; set; } = string.Empty;

        /// <summary>"mantenimiento", "instalacion" o "visita tecnica".</summary>
        public string TipoServicio { get; set; } = string.Empty;

        /// <summary>Numero de informe, o "#id" si aun no tiene numero.</summary>
        public string NumeroInforme { get; set; } = string.Empty;

        /// <summary>Quien dejo el informe listo para revisar.</summary>
        public string Tecnico { get; set; } = string.Empty;

        public string Cliente { get; set; } = string.Empty;

        /// <summary>Enlace directo al informe dentro de la aplicacion.</summary>
        public string Enlace { get; set; } = string.Empty;
    }
}
