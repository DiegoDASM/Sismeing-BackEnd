using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sismeing.Domain.Entities.Operaciones
{
    [Table("area_empresa", Schema = "public")]
    public class AreaEmpresa : AuditProperties
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("nombre_area")]
        [StringLength(255)]
        public string NombreArea { get; set; } = string.Empty;

        [Column("empresa_id")]
        public int EmpresaId { get; set; }

        // Navegación
        [ForeignKey("EmpresaId")]
        public Empresa? Empresa { get; set; }

        public ICollection<Instalacion> Instalaciones { get; set; } = [];
        public ICollection<Medicion> Mediciones { get; set; } = [];
    }
}
