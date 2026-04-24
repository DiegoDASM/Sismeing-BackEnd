using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sismeing.Domain.Entities.Operaciones
{
    [Table("foto_mantenimiento", Schema = "public")]
    public class FotoMantenimiento : AuditProperties
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("mantenimiento_id")]
        public int MantenimientoId { get; set; }

        [Required]
        [Column("url")]
        public string Url { get; set; } = string.Empty;

        [Column("tipo")]
        [StringLength(100)]
        public string? Tipo { get; set; }

        // Navegación
        [ForeignKey("MantenimientoId")]
        public Mantenimiento? Mantenimiento { get; set; }
    }
}
