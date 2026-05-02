using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sismeing.Domain.Entities.Catalogo
{
    [Table("trabajo", Schema = "public")]
    public class Trabajo : AuditCatProperties
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }
        [Column("trabajo")]
        public string NombreTrabajo { get; set; } = null!;
        [Column("descripcion")]
        public string? Descripcion { get; set; }
    }
}
