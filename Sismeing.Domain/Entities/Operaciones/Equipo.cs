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
    [Table(nameof(Equipo), Schema = "public")]
    public class Equipo : AuditCatProperties
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Nombre { get; set; } = null!;
        public int MarcaId { get; set; }
        public int TipoId { get; set; }
        public int ModeloId { get; set; }
        public string? Codigo { get; set; }
        public string? NumeroSerie { get; set; }
        public int? ProyectoId { get; set; }

        [ForeignKey("MarcaId")]
        public virtual Marca? Marca { get; set; }
        [ForeignKey("TipoId")]
        public virtual Tipo_Equipo? TipoEquipo { get; set; }
        [ForeignKey("ModeloId")]
        public virtual Modelo? Modelo { get; set; }
        [ForeignKey("ProyectoId")]
        public virtual Contrato? Proyecto { get; set; }
    }
}
