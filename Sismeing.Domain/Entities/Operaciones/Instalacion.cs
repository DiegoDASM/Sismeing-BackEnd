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
    [Table("instalacion", Schema = "public")]
    public class Instalacion : AuditCatProperties
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }
        [Column("equipo_id")]
        public int EquipoId { get; set; }
        [Column("area_id")]
        public int AreaId { get; set; }
        [Column("tecnico_id")]
        public int TecnicoId { get; set; }
        [Column("orden_trabajo")]
        public string? OrdenTrabajo { get; set; }
        [Column("horas_trabajadas")]
        public decimal? HorasTrabajadas { get; set; }
        [Column("fecha_inicio")]
        public DateTime? FechaInicio { get; set; }
        [Column("fecha_fin")]
        public DateTime? FechaFin { get; set; }
        [Column("estado_id")]
        public int EstadoId { get; set; }
        [Column("numero_informe")]
        public string? NumeroInforme { get; set; }

        [ForeignKey("EquipoId")]
        public virtual Equipo? Equipo { get; set; }
        [ForeignKey("AreaId")]
        public virtual Area_Empresa? Area { get; set; }
        [ForeignKey("TecnicoId")]
        public virtual Usuario? Tecnico { get; set; }
        [ForeignKey("EstadoId")]
        public virtual Estado? Estado { get; set; }

        // Tecnicos colaboradores (adicionales al responsable TecnicoId).
        public virtual ICollection<Instalacion_Tecnico> Colaboradores { get; set; } = new List<Instalacion_Tecnico>();

        // Ids de colaboradores que envia el formulario (no es columna). El servicio
        // los persiste en instalacion_tecnico tras guardar. Null = no tocar.
        [NotMapped]
        public List<int>? ColaboradorIds { get; set; }
    }
}
