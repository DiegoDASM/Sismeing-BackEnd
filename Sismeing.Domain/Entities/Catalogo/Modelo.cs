using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sismeing.Domain.Entities.Catalogo
{
    [Table("modelo", Schema = "public")]
    public class Modelo : AuditProperties
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("modelo")]
        [StringLength(255)]
        public string NombreModelo { get; set; } = string.Empty;

        [Column("capacidad")]
        [StringLength(255)]
        public string? Capacidad { get; set; }

        [Column("potencia")]
        [StringLength(255)]
        public string? Potencia { get; set; }

        [Column("anio_fabricacion")]
        public short? AñoFabricacion { get; set; }

        // Navegación
        public ICollection<Operaciones.Equipo> Equipos { get; set; } = [];
    }
}
