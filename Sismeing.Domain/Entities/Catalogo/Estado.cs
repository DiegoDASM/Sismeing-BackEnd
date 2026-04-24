using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sismeing.Domain.Entities.Catalogo
{
    [Table("estado", Schema = "public")]
    public class Estado : AuditProperties
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("estado")]
        [StringLength(255)]
        public string NombreEstado { get; set; } = string.Empty;

        // Navegación
        public ICollection<Operaciones.Instalacion> Instalaciones { get; set; } = [];
        public ICollection<Operaciones.Mantenimiento> Mantenimientos { get; set; } = [];
    }
}
