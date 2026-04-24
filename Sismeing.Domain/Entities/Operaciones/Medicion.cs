using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sismeing.Domain.Entities.Operaciones
{
    [Table("medicion", Schema = "public")]
    public class Medicion : AuditProperties
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("voltaje")]
        public decimal? Voltaje { get; set; }

        [Column("frecuencia")]
        public decimal? Frecuencia { get; set; }

        [Column("amp_evaporador_ventilador_rla")]
        public decimal? AmpEvaporadorVentiladorRla { get; set; }

        [Column("amp_motor_condensadora_rla")]
        public decimal? AmpMotorCondensadoraRla { get; set; }

        [Column("amp_compresor_rla")]
        public decimal? AmpCompresorRla { get; set; }

        [Column("presion_succion_psi")]
        public decimal? PresionSuccionPsi { get; set; }

        [Column("presion_descarga_psi")]
        public decimal? PresionDescargaPsi { get; set; }

        [Column("temp_inicial_final_evap_c")]
        public decimal? TempInicialFinalEvapC { get; set; }

        [Column("temp_inicial_final_cond_c")]
        public decimal? TempInicialFinalCondC { get; set; }

        [Column("temp_ingreso_salida_agua_c")]
        public decimal? TempIngresoSalidaAguaC { get; set; }

        [Column("temperatura_programada_c")]
        public decimal? TemperaturaProgramadaC { get; set; }

        [Column("humedad_relativa_prog_pct")]
        public decimal? HumedadRelativaProgPct { get; set; }

        [Column("equipo_id")]
        public int EquipoId { get; set; }

        [Column("informe_id")]
        public int? InformeId { get; set; }

        [Column("area_id")]
        public int AreaId { get; set; }

        [Column("inicial")]
        public bool Inicial { get; set; } = true;

        // Navegación
        [ForeignKey("EquipoId")]
        public Equipo? Equipo { get; set; }

        [ForeignKey("AreaId")]
        public AreaEmpresa? Area { get; set; }
    }
}
