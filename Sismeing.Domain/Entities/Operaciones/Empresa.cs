using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sismeing.Domain.Entities.Operaciones
{
    [Table(nameof(Empresa), Schema = "public")]
    public class Empresa : AuditCatProperties
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Nombre { get; set; } = null!;
        public string RazonSocial { get; set; } = null!;
        public string? Telefono { get; set; }
        public string? CorreoElectronico { get; set; }
        public string? Logo { get; set; }
    }
}
