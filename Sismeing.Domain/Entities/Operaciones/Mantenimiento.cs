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
    [Table("mantenimiento", Schema = "public")]
    public class Mantenimiento : AuditCatProperties
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }

        // El mantenimiento pertenece a un EQUIPO. La instalación es opcional:
        // hay equipos que ya estaban instalados y solo reciben mantenimiento.
        [Column("equipo_id")]
        public int? EquipoId { get; set; }

        [Column("instalacion_id")]
        public int? InstalacionId { get; set; }

        [Column("tecnico_id")]
        public int TecnicoId { get; set; }

        [Column("observacion_inicial")]
        public string? ObservacionInicial { get; set; }

        [Column("observaciones_finales")]
        public string? ObservacionesFinales { get; set; }

        [Column("requiere_repuestos")]
        public bool RequiereRepuestos { get; set; }

        [Column("tipo_mantenimiento_id")]
        public int TipoMantenimientoId { get; set; }

        [Column("fecha_inicio")]
        public DateTime? FechaInicio { get; set; }

        [Column("fecha_fin")]
        public DateTime? FechaFin { get; set; }

        [Column("fecha_proximo")]
        public DateTime? FechaProximo { get; set; }

        [Column("estado_id")]
        public int EstadoId { get; set; }

        [Column("supervisor_id")]
        public int? SupervisorId { get; set; }

        [Column("encargado_id")]
        public int? EncargadoId { get; set; }

        [Column("numero_informe")]
        public string? NumeroInforme { get; set; }

        [NotMapped]
        public bool EnviarCorreoRecordatorio { get; set; }

        [ForeignKey("EquipoId")]
        public virtual Equipo? Equipo { get; set; }
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

        // Tecnicos colaboradores (adicionales al responsable TecnicoId).
        public virtual ICollection<Mantenimiento_Tecnico> Colaboradores { get; set; } = new List<Mantenimiento_Tecnico>();

        // Ids de colaboradores que envia el formulario (no es columna). El servicio
        // los persiste en mantenimiento_tecnico tras guardar. Null = no tocar.
        [NotMapped]
        public List<int>? ColaboradorIds { get; set; }
    }
}
