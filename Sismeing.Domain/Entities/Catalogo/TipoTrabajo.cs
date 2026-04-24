using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sismeing.Domain.Entities.Catalogo
{
    [Table("tipo_trabajo", Schema = "public")]
    public class TipoTrabajo : AuditProperties
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("tipo_trabajo")]
        [StringLength(255)]
        public string NombreTipo { get; set; } = string.Empty;

        // Navegación
        public ICollection<Operaciones.Contrato> Contratos { get; set; } = [];
        public ICollection<Operaciones.VisitaTecnica> VisitasTecnicas { get; set; } = [];
    }
}
