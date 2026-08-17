using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sismeing.Domain.Entities.Operaciones
{
    [Table("medicion", Schema = "public")]
    public class Medicion : AuditCatProperties
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }
        // Texto libre a proposito: en campo se registran valores como "220/110"
        // (voltaje bifasico) o "12/24" (ingreso/salida), que un decimal rechazaba.
        [Column("voltaje")]
        public string? Voltaje { get; set; }
        [Column("frecuencia")]
        public string? Frecuencia { get; set; }
        [Column("amp_evaporador_ventilador_rla")]
        public string? AmpEvaporadorVentiladorRla { get; set; }
        [Column("amp_motor_condensadora_rla")]
        public string? AmpMotorCondensadoraRla { get; set; }
        [Column("amp_compresor_rla")]
        public string? AmpCompresorRla { get; set; }
        [Column("presion_succion_psi")]
        public string? PresionSuccionPsi { get; set; }
        [Column("presion_descarga_psi")]
        public string? PresionDescargaPsi { get; set; }
        [Column("temp_inicial_final_evap_c")]
        public string? TempInicialFinalEvapC { get; set; }
        [Column("temp_inicial_final_cond_c")]
        public string? TempInicialFinalCondC { get; set; }
        [Column("temp_ingreso_salida_agua_c")]
        public string? TempIngresoSalidaAguaC { get; set; }
        [Column("temperatura_programada_c")]
        public string? TemperaturaProgramadaC { get; set; }
        [Column("humedad_relativa_prog_pct")]
        public string? HumedadRelativaProgPct { get; set; }
        [Column("equipo_id")]
        public int EquipoId { get; set; }
        [Column("informe_id")]
        public int? InformeId { get; set; }
        // Distingue si el informe_id es de una instalacion o de un mantenimiento.
        [Column("origen")]
        public string? Origen { get; set; }
        [Column("area_id")]
        public int AreaId { get; set; }
        [Column("inicial")]
        public bool Inicial { get; set; }

        [ForeignKey("EquipoId")]
        public virtual Equipo? Equipo { get; set; }
        [ForeignKey("AreaId")]
        public virtual Area_Empresa? Area { get; set; }
    }
}
