using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sismeing.Domain.Entities.Catalogo;

namespace Sismeing.Domain.Entities.Operaciones
{
    [Table(nameof(Visita_Tecnica), Schema = "public")]
    public class Visita_Tecnica : AuditCatProperties
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int EmpresaId { get; set; }
        public int TecnicoId { get; set; }
        public int TipoTrabajoId { get; set; }
        public DateTime FechaVisita { get; set; }
        public string? DescripcionVisita { get; set; }
        public string? Observaciones { get; set; }
        public string? NumeroInforme { get; set; }

        [ForeignKey("EmpresaId")]
        public virtual Empresa? Empresa { get; set; }
        [ForeignKey("TecnicoId")]
        public virtual Usuario? Tecnico { get; set; }
        [ForeignKey("TipoTrabajoId")]
        public virtual Tipo_Trabajo? TipoTrabajo { get; set; }
    }
}
