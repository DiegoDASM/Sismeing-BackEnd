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
    [Table("visita_tecnica", Schema = "public")]
    public class Visita_Tecnica : AuditCatProperties
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }
        [Column("empresa_id")]
        public int EmpresaId { get; set; }
        [Column("tecnico_id")]
        public int TecnicoId { get; set; }
        [Column("tipo_trabajo_id")]
        public int TipoTrabajoId { get; set; }
        [Column("fecha_visita")]
        public DateTime FechaVisita { get; set; }
        [Column("descripcion_visita")]
        public string? DescripcionVisita { get; set; }
        [Column("observaciones")]
        public string? Observaciones { get; set; }
        [Column("numero_informe")]
        public string? NumeroInforme { get; set; }

        [ForeignKey("EmpresaId")]
        public virtual Empresa? Empresa { get; set; }
        [ForeignKey("TecnicoId")]
        public virtual Usuario? Tecnico { get; set; }
        [ForeignKey("TipoTrabajoId")]
        public virtual Tipo_Trabajo? TipoTrabajo { get; set; }
    }
}
