using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Sismeing.Domain.Entities.Catalogo;

namespace Sismeing.Domain.Entities.Operaciones
{
    // Tipo de trabajo que cubre un contrato (Instalacion y/o Mantenimiento).
    // Un contrato puede tener varias filas (una por tipo).
    [Table("contrato_tipo_trabajo", Schema = "public")]
    public class Contrato_TipoTrabajo
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }
        [Column("contrato_id")]
        public int ContratoId { get; set; }
        [Column("tipo_trabajo_id")]
        public int TipoTrabajoId { get; set; }

        [ForeignKey("ContratoId")]
        public virtual Contrato? Contrato { get; set; }
        [ForeignKey("TipoTrabajoId")]
        public virtual Tipo_Trabajo? TipoTrabajo { get; set; }
    }
}
