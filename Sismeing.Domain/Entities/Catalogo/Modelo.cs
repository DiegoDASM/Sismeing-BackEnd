using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sismeing.Domain.Entities.Catalogo
{
    [Table("modelo", Schema = "public")]
    public class Modelo : AuditCatProperties
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }
        [Column("modelo")]
        public string Nombre { get; set; } = null!;
        [Column("capacidad")]
        public string? Capacidad { get; set; }
        [Column("potencia")]
        public string? Potencia { get; set; }
        [Column("anio_fabricacion")]
        public short? AnioFabricacion { get; set; }

        // Tipo de equipo al que pertenece el modelo. Opcional: los modelos
        // anteriores a esta columna quedan sin tipo hasta asignárselo.
        [Column("tipo_id")]
        public int? TipoId { get; set; }

        [ForeignKey("TipoId")]
        public virtual Tipo_Equipo? TipoEquipo { get; set; }
    }
}
