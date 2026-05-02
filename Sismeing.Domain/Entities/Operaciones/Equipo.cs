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
    [Table("equipo", Schema = "public")]
    public class Equipo : AuditCatProperties
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }

        [Column("nombre")]
        public string Nombre { get; set; } = null!;

        [Column("marca_id")]
        public int MarcaId { get; set; }

        [Column("tipo_id")]
        public int TipoId { get; set; }

        [Column("modelo_id")]
        public int ModeloId { get; set; }

        [Column("codigo")]
        public string? Codigo { get; set; }

        [Column("numero_serie")]
        public string? NumeroSerie { get; set; }

        [Column("proyecto_id")]
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
