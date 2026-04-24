using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Sismeing.Domain.Entities
{
    public abstract class AuditProperties
    {
        public bool Activo { get; set; } = true;

        [StringLength(100)]
        public string? UsuarioRegistro { get; set; } = "SYSTEM";

        public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;

        [JsonIgnore]
        [StringLength(50)]
        public string? IpRegistro { get; set; }

        [JsonIgnore]
        [StringLength(100)]
        public string? UsuarioModificacion { get; set; }

        [JsonIgnore]
        public DateTime? FechaModificacion { get; set; }

        [JsonIgnore]
        [StringLength(50)]
        public string? IpModificacion { get; set; }

        [JsonIgnore]
        [StringLength(100)]
        public string? UsuarioEliminacion { get; set; }

        [JsonIgnore]
        public DateTime? FechaEliminacion { get; set; }

        [JsonIgnore]
        [StringLength(50)]
        public string? IpEliminacion { get; set; }
    }
}
