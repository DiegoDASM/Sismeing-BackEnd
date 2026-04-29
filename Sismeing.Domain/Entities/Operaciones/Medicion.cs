using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sismeing.Domain.Entities.Operaciones
{
    [Table(nameof(Medicion), Schema = "public")]
    public class Medicion : AuditCatProperties
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public decimal? Voltaje { get; set; }
        public decimal? Frecuencia { get; set; }
        public decimal? AmpEvaporadorVentiladorRla { get; set; }
        public decimal? AmpMotorCondensadoraRla { get; set; }
        public decimal? AmpCompresorRla { get; set; }
        public decimal? PresionSuccionPsi { get; set; }
        public decimal? PresionDescargaPsi { get; set; }
        public decimal? TempInicialFinalEvapC { get; set; }
        public decimal? TempInicialFinalCondC { get; set; }
        public decimal? TempIngresoSalidaAguaC { get; set; }
        public decimal? TemperaturaProgramadaC { get; set; }
        public decimal? HumedadRelativaProgPct { get; set; }
        public int EquipoId { get; set; }
        public int? InformeId { get; set; }
        public int AreaId { get; set; }
        public bool Inicial { get; set; }

        [ForeignKey("EquipoId")]
        public virtual Equipo? Equipo { get; set; }
        [ForeignKey("AreaId")]
        public virtual Area_Empresa? Area { get; set; }
    }
}
