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
    [Table(nameof(Contrato), Schema = "public")]
    public class Contrato : AuditCatProperties
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string NombreProyecto { get; set; } = null!;
        public int EmpresaId { get; set; }
        public int DireccionId { get; set; }
        public int EncargadoId { get; set; }
        public int TipoTrabajoId { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }

        [ForeignKey("EmpresaId")]
        public virtual Empresa? Empresa { get; set; }
        [ForeignKey("DireccionId")]
        public virtual Direccion_Empresa? Direccion { get; set; }
        [ForeignKey("EncargadoId")]
        public virtual Usuario? Encargado { get; set; }
        [ForeignKey("TipoTrabajoId")]
        public virtual Tipo_Trabajo? TipoTrabajo { get; set; }
    }
}
