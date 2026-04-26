using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sismeing.Domain.Entities.Catalogo
{
    [Table("rol", Schema = "public")]
    public class Rol : AuditProperties
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("rol")]
        [StringLength(255)]
        public string NombreRol { get; set; } = string.Empty;

        // Navegación
        public ICollection<Operaciones.Usuario> Usuarios { get; set; } = [];
    }
}
