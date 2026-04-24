using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sismeing.Domain.Entities.Operaciones
{
    [Table("mantenimiento", Schema = "public")]
    public class Mantenimiento : AuditProperties
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("instalacion_id")]
        public int InstalacionId { get; set; }

        [Column("tecnico_id")]
        public int TecnicoId { get; set; }

        [Column("observacion_inicial")]
        public string? ObservacionInicial { get; set; }

        [Column("observaciones_finales")]
        public string? ObservacionesFinales { get; set; }

        [Column("requiere_repuestos")]
        public bool RequiereRepuestos { get; set; } = false;

        [Column("tipo_mantenimiento_id")]
        public int TipoMantenimientoId { get; set; }

        [Column("fecha_inicio")]
        public DateTime? FechaInicio { get; set; }

        [Column("fecha_fin")]
        public DateTime? FechaFin { get; set; }

        [Column("fecha_proximo")]
        public DateOnly? FechaProximo { get; set; }

        [Column("estado_id")]
        public int EstadoId { get; set; }

        [Column("supervisor_id")]
        public int? SupervisorId { get; set; }

        [Column("encargado_id")]
        public int? EncargadoId { get; set; }

        [Column("numero_informe")]
        [StringLength(100)]
        public string? NumeroInforme { get; set; }

        // Navegación
        [ForeignKey("InstalacionId")]
        public Instalacion? Instalacion { get; set; }

        [ForeignKey("TecnicoId")]
        public Usuario? Tecnico { get; set; }

        [ForeignKey("TipoMantenimientoId")]
        public Catalogo.TipoMantenimiento? TipoMantenimiento { get; set; }

        [ForeignKey("EstadoId")]
        public Catalogo.Estado? Estado { get; set; }

        [ForeignKey("SupervisorId")]
        public Usuario? Supervisor { get; set; }

        [ForeignKey("EncargadoId")]
        public Usuario? Encargado { get; set; }

        public ICollection<FotoMantenimiento> Fotos { get; set; } = [];
    }
}
