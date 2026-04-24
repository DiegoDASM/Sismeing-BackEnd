using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sismeing.Domain.Entities.Operaciones
{
    [Table("foto_instalacion", Schema = "public")]
    public class FotoInstalacion : AuditProperties
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("instalacion_id")]
        public int InstalacionId { get; set; }

        [Required]
        [Column("url")]
        public string Url { get; set; } = string.Empty;

        [Column("tipo")]
        [StringLength(100)]
        public string? Tipo { get; set; }

        // Navegación
        [ForeignKey("InstalacionId")]
        public Instalacion? Instalacion { get; set; }
    }
}
