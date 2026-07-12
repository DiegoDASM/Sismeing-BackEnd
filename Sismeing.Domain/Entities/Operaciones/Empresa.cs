using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sismeing.Domain.Entities.Operaciones
{
    [Table("empresa", Schema = "public")]
    public class Empresa : AuditCatProperties
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }

        [Column("nombre")]
        public string Nombre { get; set; } = null!;

        [Column("razon_social")]
        public string RazonSocial { get; set; } = null!;

        [Column("ruc")]
        [StringLength(13)]
        public string? Ruc { get; set; }

        [Column("telefono")]
        public string? Telefono { get; set; }

        [Column("correo_electronico")]
        public string? CorreoElectronico { get; set; }

        [Column("logo")]
        public string? Logo { get; set; }

        // Contador secuencial de informes por empresa (instalación y mantenimiento).
        [System.Text.Json.Serialization.JsonIgnore]
        [Column("numero_informe_seq")]
        public int NumeroInformeSeq { get; set; }
    }
}
