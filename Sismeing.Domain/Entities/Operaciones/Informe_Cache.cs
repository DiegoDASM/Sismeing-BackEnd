using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sismeing.Domain.Entities.Operaciones
{
    /// <summary>
    /// HTML ya generado de un informe. Antes se renderizaba de cero en cada
    /// visualizacion (consultando fotos, mediciones, trabajos y recomponiendo
    /// la plantilla); ahora se genera una vez y se reutiliza.
    ///
    /// La invalidacion NO usa tiempo de expiracion: se compara
    /// <see cref="FechaGeneracion"/> con la fecha_modificacion del servicio de
    /// origen. Si el servicio cambio despues de generarse, se vuelve a generar.
    /// Asi el informe siempre refleja el estado real, sin trabajo de mas.
    ///
    /// Tabla aparte a proposito: no toca ninguna tabla existente, de modo que
    /// la migracion en produccion solo anade y no puede romper nada.
    /// </summary>
    [Table("informe_cache", Schema = "public")]
    public class Informe_Cache
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }

        /// <summary>
        /// Variante del informe: 'instalacion-datos', 'instalacion-fotografico',
        /// 'mantenimiento-datos', 'mantenimiento-fotografico', 'visita-datos',
        /// 'visita-fotografico', 'equipo-hojavida'.
        /// </summary>
        [Column("tipo")]
        public string Tipo { get; set; } = string.Empty;

        /// <summary>Id del servicio o equipo al que pertenece el informe.</summary>
        [Column("referencia_id")]
        public int ReferenciaId { get; set; }

        [Column("html")]
        public string Html { get; set; } = string.Empty;

        [Column("fecha_generacion")]
        public DateTime FechaGeneracion { get; set; }
    }
}
