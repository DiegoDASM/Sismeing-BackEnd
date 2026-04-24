using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sismeing.Domain.Entities.Operaciones
{
    [Table("direccion_empresa", Schema = "public")]
    public class DireccionEmpresa : AuditProperties
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("empresa_id")]
        public int EmpresaId { get; set; }

        [Required]
        [Column("direccion")]
        [StringLength(500)]
        public string Direccion { get; set; } = string.Empty;

        // Navegación
        [ForeignKey("EmpresaId")]
        public Empresa? Empresa { get; set; }

        public ICollection<Contrato> Contratos { get; set; } = [];
    }
}
