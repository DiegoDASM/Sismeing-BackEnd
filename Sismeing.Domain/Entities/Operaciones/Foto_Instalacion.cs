using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sismeing.Domain.Entities.Operaciones
{
    [Table("foto_instalacion", Schema = "public")]
    public class Foto_Instalacion : AuditCatProperties
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }

        [Column("instalacion_id")]
        public int InstalacionId { get; set; }

        [Column("url")]
        public string Url { get; set; } = null!;

        [Column("tipo")]
        public string? Tipo { get; set; }

        [ForeignKey("InstalacionId")]
        public virtual Instalacion? Instalacion { get; set; }
    }
}
