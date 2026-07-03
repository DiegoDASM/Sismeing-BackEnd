using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sismeing.Domain.Entities.Operaciones
{
    [Table("foto_visita_tecnica", Schema = "public")]
    public class Foto_VisitaTecnica : AuditCatProperties
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }

        [Column("visita_tecnica_id")]
        public int VisitaTecnicaId { get; set; }

        [Column("url")]
        public string Url { get; set; } = null!;

        [Column("tipo")]
        public string? Tipo { get; set; }

        [ForeignKey("VisitaTecnicaId")]
        public virtual Visita_Tecnica? VisitaTecnica { get; set; }
    }
}
