using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sismeing.Domain.Entities.Catalogo
{
    [Table("tipo_mantenimiento", Schema = "public")]
    public class TipoMantenimiento : AuditProperties
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("tipo_mantenimiento")]
        [StringLength(255)]
        public string NombreTipo { get; set; } = string.Empty;

        // Navegación
        public ICollection<Operaciones.Mantenimiento> Mantenimientos { get; set; } = [];
    }
}
