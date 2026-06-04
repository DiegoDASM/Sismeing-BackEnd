using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sismeing.Domain.Entities.Catalogo
{
    [Table("tipo_equipo", Schema = "public")]
    public class Tipo_Equipo : AuditCatProperties
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }
        [Column("equipo")]
        public string Nombre { get; set; } = null!;
    }
}
