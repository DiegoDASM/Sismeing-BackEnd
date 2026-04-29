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
    [Table(nameof(Instalacion), Schema = "public")]
    public class Instalacion : AuditCatProperties
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int EquipoId { get; set; }
        public int AreaId { get; set; }
        public int TecnicoId { get; set; }
        public string? OrdenTrabajo { get; set; }
        public decimal? HorasTrabajadas { get; set; }
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public int EstadoId { get; set; }
        public string? NumeroInforme { get; set; }

        [ForeignKey("EquipoId")]
        public virtual Equipo? Equipo { get; set; }
        [ForeignKey("AreaId")]
        public virtual Area_Empresa? Area { get; set; }
        [ForeignKey("TecnicoId")]
        public virtual Usuario? Tecnico { get; set; }
        [ForeignKey("EstadoId")]
        public virtual Estado? Estado { get; set; }
    }
}
