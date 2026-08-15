using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sismeing.Domain.Entities.Catalogo
{
    [Table("trabajo", Schema = "public")]
    public class Trabajo : AuditCatProperties
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }
        [Column("trabajo")]
        public string NombreTrabajo { get; set; } = null!;
        [Column("descripcion")]
        public string? Descripcion { get; set; }

        // Trabajo realizado dentro de un mantenimiento (nullable: el catálogo base no lo lleva).
        [Column("mantenimiento_id")]
        public int? MantenimientoId { get; set; }

        // Excluyente con MantenimientoId. Con ambos en NULL el registro es una
        // plantilla del catalogo reutilizable, no un trabajo ya realizado.
        [Column("instalacion_id")]
        public int? InstalacionId { get; set; }

        [ForeignKey("MantenimientoId")]
        public virtual Operaciones.Mantenimiento? Mantenimiento { get; set; }
    }
}
