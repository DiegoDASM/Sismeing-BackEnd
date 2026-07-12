using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sismeing.Domain.Entities.Operaciones
{
    [Table("foto_mantenimiento", Schema = "public")]
    public class Foto_Mantenimiento : AuditCatProperties
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }

        [Column("mantenimiento_id")]
        public int MantenimientoId { get; set; }

        [Column("url")]
        public string Url { get; set; } = null!;

        [Column("tipo")]
        public string? Tipo { get; set; }

        [ForeignKey("MantenimientoId")]
        public virtual Mantenimiento? Mantenimiento { get; set; }
    }
}
