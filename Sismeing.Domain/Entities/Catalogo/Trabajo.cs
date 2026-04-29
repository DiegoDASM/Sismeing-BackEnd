using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sismeing.Domain.Entities.Catalogo
{
    [Table(nameof(Trabajo), Schema = "public")]
    public class Trabajo : AuditCatProperties
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string NombreTrabajo { get; set; } = null!;
        public string? Descripcion { get; set; }
    }
}
