using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sismeing.Domain.Entities.Operaciones
{
    [Table(nameof(Direccion_Empresa), Schema = "public")]
    public class Direccion_Empresa : AuditCatProperties
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int EmpresaId { get; set; }
        public string Direccion { get; set; } = null!;

        [ForeignKey("EmpresaId")]
        public virtual Empresa? Empresa { get; set; }
    }
}
