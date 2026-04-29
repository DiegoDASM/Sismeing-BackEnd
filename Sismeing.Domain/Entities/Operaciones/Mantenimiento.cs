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
    [Table(nameof(Mantenimiento), Schema = "public")]
    public class Mantenimiento : AuditCatProperties
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int InstalacionId { get; set; }
        public int TecnicoId { get; set; }
        public string? ObservacionInicial { get; set; }
        public string? ObservacionesFinales { get; set; }
        public bool RequiereRepuestos { get; set; }
        public int TipoMantenimientoId { get; set; }
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public DateTime? FechaProximo { get; set; }
        public int EstadoId { get; set; }
        public int? SupervisorId { get; set; }
        public int? EncargadoId { get; set; }
        public string? NumeroInforme { get; set; }

        [ForeignKey("InstalacionId")]
        public virtual Instalacion? Instalacion { get; set; }
        [ForeignKey("TecnicoId")]
        public virtual Usuario? Tecnico { get; set; }
        [ForeignKey("TipoMantenimientoId")]
        public virtual Tipo_Mantenimiento? TipoMantenimiento { get; set; }
        [ForeignKey("EstadoId")]
        public virtual Estado? Estado { get; set; }
        [ForeignKey("SupervisorId")]
        public virtual Usuario? Supervisor { get; set; }
        [ForeignKey("EncargadoId")]
        public virtual Usuario? Encargado { get; set; }
    }
}
