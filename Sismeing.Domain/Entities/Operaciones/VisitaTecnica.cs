using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sismeing.Domain.Entities.Operaciones
{
    [Table("visita_tecnica", Schema = "public")]
    public class VisitaTecnica : AuditProperties
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("empresa_id")]
        public int EmpresaId { get; set; }

        [Column("tecnico_id")]
        public int TecnicoId { get; set; }

        [Column("tipo_trabajo_id")]
        public int TipoTrabajoId { get; set; }

        [Required]
        [Column("fecha_visita")]
        public DateOnly FechaVisita { get; set; }

        [Column("descripcion_visita")]
        public string? DescripcionVisita { get; set; }

        [Column("observaciones")]
        public string? Observaciones { get; set; }

        [Column("numero_informe")]
        [StringLength(100)]
        public string? NumeroInforme { get; set; }

        // Navegación
        [ForeignKey("EmpresaId")]
        public Empresa? Empresa { get; set; }

        [ForeignKey("TecnicoId")]
        public Usuario? Tecnico { get; set; }

        [ForeignKey("TipoTrabajoId")]
        public Catalogo.TipoTrabajo? TipoTrabajo { get; set; }
    }
}
