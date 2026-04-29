using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sismeing.Domain.Entities.Operaciones
{
    [Table(nameof(Foto_Instalacion), Schema = "public")]
    public class Foto_Instalacion : AuditCatProperties
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int InstalacionId { get; set; }
        public string Url { get; set; } = null!;
        public string? Tipo { get; set; }

        [ForeignKey("InstalacionId")]
        public virtual Instalacion? Instalacion { get; set; }
    }
}
