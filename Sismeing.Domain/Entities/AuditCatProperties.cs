using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Sismeing.Domain.Entities
{
    public class AuditCatProperties
    {
        public bool Activo { get; set; }

        [Required]
        [StringLength(100)]
        public string UsuarioRegistro { get; set; } = "SYSTEM";

        [Required]
        public DateTime FechaRegistro { get; set; }

        [JsonIgnore]
        [StringLength(50)]
        public string? IpRegistro { get; set; }

        [JsonIgnore]
        [StringLength(100)]
        public string? UsuarioModificacion { get; set; }

        [JsonIgnore]
        [StringLength(100)]
        public DateTime? FechaModificacion { get; set; }

        [JsonIgnore]
        [StringLength(50)]
        public string? IpModificacion { get; set; }

        [StringLength(100)]
        public string? UsuarioEliminacion { get; set; }
        [JsonIgnore]
        public DateTime? FechaEliminacion { get; set; }
        [JsonIgnore]

        [StringLength(50)]
        public string? IpEliminacion { get; set; }
    }
}
