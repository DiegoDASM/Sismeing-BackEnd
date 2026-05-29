using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sismeing.Domain.Entities.Operaciones
{
    [Table("area_empresa", Schema = "public")]
    public class Area_Empresa : AuditCatProperties
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }

        [Column("nombre_area")]
        public string NombreArea { get; set; } = null!;

        [Column("empresa_id")]
        public int EmpresaId { get; set; }

        [Column("direccion_id")]
        public int? DireccionEmpresaId { get; set; }

        [ForeignKey("EmpresaId")]
        public virtual Empresa? Empresa { get; set; }

        [ForeignKey("DireccionEmpresaId")]
        public virtual Direccion_Empresa? DireccionEmpresa { get; set; }
    }
}
