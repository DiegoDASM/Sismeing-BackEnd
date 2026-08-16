using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Sismeing.Domain.Entities
{
    public class AuditCatProperties
    {
        [Column("activo")]
        public bool Activo { get; set; }
        //[Required]
        [StringLength(100)]
        [Column("UsuarioRegistro")]
        public string? UsuarioRegistro { get; set; }
        [Required]
        [Column("FechaRegistro")]
        public DateTime FechaRegistro { get; set; }
        [JsonIgnore]
        [StringLength(50)]
        [Column("IpRegistro")]
        public string? IpRegistro { get; set; }
        [JsonIgnore]
        [StringLength(100)]
        [Column("UsuarioModificacion")]
        public string? UsuarioModificacion { get; set; }
        [JsonIgnore]
        [Column("FechaModificacion")]
        public DateTime? FechaModificacion { get; set; }
        [JsonIgnore]
        [StringLength(50)]
        [Column("IpModificacion")]
        public string? IpModificacion { get; set; }
        [StringLength(100)]
        [Column("UsuarioEliminacion")]
        public string? UsuarioEliminacion { get; set; }

        [JsonIgnore]
        [Column("FechaEliminacion")]
        public DateTime? FechaEliminacion { get; set; }

        [JsonIgnore]
        [StringLength(50)]
        [Column("IpEliminacion")]
        public string? IpEliminacion { get; set; }
    }
}
