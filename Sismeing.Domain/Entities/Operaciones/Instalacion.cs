using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sismeing.Domain.Entities.Operaciones
{
    [Table("instalacion", Schema = "public")]
    public class Instalacion : AuditProperties
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("equipo_id")]
        public int EquipoId { get; set; }

        [Column("area_id")]
        public int AreaId { get; set; }

        [Column("tecnico_id")]
        public int TecnicoId { get; set; }

        [Column("orden_trabajo")]
        [StringLength(100)]
        public string? OrdenTrabajo { get; set; }

        [Column("horas_trabajadas")]
        public decimal? HorasTrabajadas { get; set; }

        [Column("fecha_inicio")]
        public DateTime? FechaInicio { get; set; }

        [Column("fecha_fin")]
        public DateTime? FechaFin { get; set; }

        [Column("estado_id")]
        public int EstadoId { get; set; }

        [Column("numero_informe")]
        [StringLength(100)]
        public string? NumeroInforme { get; set; }

        // Navegación
        [ForeignKey("EquipoId")]
        public Equipo? Equipo { get; set; }

        [ForeignKey("AreaId")]
        public AreaEmpresa? Area { get; set; }

        [ForeignKey("TecnicoId")]
        public Usuario? Tecnico { get; set; }

        [ForeignKey("EstadoId")]
        public Catalogo.Estado? Estado { get; set; }

        public ICollection<FotoInstalacion> Fotos { get; set; } = [];
        public ICollection<Mantenimiento> Mantenimientos { get; set; } = [];
    }
}
