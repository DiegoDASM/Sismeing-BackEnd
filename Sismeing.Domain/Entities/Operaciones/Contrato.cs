using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sismeing.Domain.Entities.Operaciones
{
    [Table("contrato", Schema = "public")]
    public class Contrato : AuditProperties
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("nombre_proyecto")]
        [StringLength(255)]
        public string NombreProyecto { get; set; } = string.Empty;

        [Column("empresa_id")]
        public int EmpresaId { get; set; }

        [Column("direccion_id")]
        public int DireccionId { get; set; }

        [Column("encargado_id")]
        public int EncargadoId { get; set; }

        [Column("tipo_trabajo_id")]
        public int TipoTrabajoId { get; set; }

        [Required]
        [Column("fecha_inicio")]
        public DateOnly FechaInicio { get; set; }

        [Column("fecha_fin")]
        public DateOnly? FechaFin { get; set; }

        // Navegación
        [ForeignKey("EmpresaId")]
        public Empresa? Empresa { get; set; }

        [ForeignKey("DireccionId")]
        public DireccionEmpresa? Direccion { get; set; }

        [ForeignKey("EncargadoId")]
        public Usuario? Encargado { get; set; }

        [ForeignKey("TipoTrabajoId")]
        public Catalogo.TipoTrabajo? TipoTrabajo { get; set; }

        public ICollection<Equipo> Equipos { get; set; } = [];
    }
}
