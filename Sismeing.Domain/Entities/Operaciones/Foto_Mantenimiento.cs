using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sismeing.Domain.Entities.Operaciones
{
    [Table(nameof(Foto_Mantenimiento), Schema = "public")]
    public class Foto_Mantenimiento : AuditProperties
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int MantenimientoId { get; set; }
        public string Url { get; set; } = null!;
        public string? Tipo { get; set; }

        [ForeignKey("MantenimientoId")]
        public virtual Mantenimiento? Mantenimiento { get; set; }
    }
}
