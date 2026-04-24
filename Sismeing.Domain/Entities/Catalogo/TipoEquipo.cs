using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sismeing.Domain.Entities.Catalogo
{
    [Table("tipo_equipo", Schema = "public")]
    public class TipoEquipo : AuditProperties
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("equipo")]
        [StringLength(255)]
        public string NombreTipo { get; set; } = string.Empty;

        // Navegación
        public ICollection<Operaciones.Equipo> Equipos { get; set; } = [];
    }
}
