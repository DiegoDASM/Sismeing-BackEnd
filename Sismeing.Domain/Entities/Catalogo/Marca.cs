using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sismeing.Domain.Entities.Catalogo
{
    [Table("marca", Schema = "public")]
    public class Marca : AuditProperties
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("marca")]
        [StringLength(255)]
        public string NombreMarca { get; set; } = string.Empty;

        // Navegación
        public ICollection<Operaciones.Equipo> Equipos { get; set; } = [];
    }
}
