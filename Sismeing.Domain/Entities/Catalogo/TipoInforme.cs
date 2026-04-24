using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sismeing.Domain.Entities.Catalogo
{
    [Table("tipo_informe", Schema = "public")]
    public class TipoInforme : AuditProperties
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("informe")]
        [StringLength(255)]
        public string NombreTipo { get; set; } = string.Empty;
    }
}
