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
    [Table("contrato", Schema = "public")]
    public class Contrato : AuditCatProperties
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }
        [Column("nombre_proyecto")]
        public string NombreProyecto { get; set; } = null!;

        [Column("empresa_id")]
        public int EmpresaId { get; set; }
        
        [Column("direccion_id")]
        public int DireccionId { get; set; }
        
        [Column("encargado_id")]
        public int EncargadoId { get; set; }
        
        [Column("tipo_trabajo_id")]
        public int TipoTrabajoId { get; set; }
        
        [Column("fecha_inicio")]
        public DateTime FechaInicio { get; set; }
        
        [Column("fecha_fin")]
        public DateTime? FechaFin { get; set; }

        [ForeignKey("EmpresaId")]
        public virtual Empresa? Empresa { get; set; }
        [ForeignKey("DireccionId")]
        public virtual Direccion_Empresa? Direccion { get; set; }
        [ForeignKey("EncargadoId")]
        public virtual Usuario? Encargado { get; set; }
        [ForeignKey("TipoTrabajoId")]
        public virtual Tipo_Trabajo? TipoTrabajo { get; set; }

        // Conjunto de tipos de trabajo que cubre el contrato (Instalacion y/o
        // Mantenimiento). TipoTrabajoId queda como tipo primario/compatibilidad.
        public virtual ICollection<Contrato_TipoTrabajo> TiposTrabajo { get; set; } = new List<Contrato_TipoTrabajo>();

        // Ids de tipos de trabajo que envia el formulario (no es columna). El
        // servicio los persiste en contrato_tipo_trabajo tras guardar.
        [NotMapped]
        public List<int>? TipoTrabajoIds { get; set; }
    }
}
