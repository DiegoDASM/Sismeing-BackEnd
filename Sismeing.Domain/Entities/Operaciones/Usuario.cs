using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Sismeing.Domain.Entities.Operaciones
{
    [Table("usuario", Schema = "public")]
    public class Usuario : AuditProperties
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("nombre")]
        [StringLength(255)]
        public string Nombre { get; set; } = string.Empty;

        [Required]
        [Column("apellido")]
        [StringLength(255)]
        public string Apellido { get; set; } = string.Empty;

        [Required]
        [Column("cedula")]
        [StringLength(50)]
        public string Cedula { get; set; } = string.Empty;

        [Required]
        [Column("correo_electronico")]
        [StringLength(255)]
        [EmailAddress]
        public string CorreoElectronico { get; set; } = string.Empty;

        [Column("telefono")]
        [StringLength(50)]
        public string? Telefono { get; set; }

        [Column("verificado")]
        public bool Verificado { get; set; } = false;

        /// <summary>Hash de la contraseña. Campo agregado a la tabla mediante migración.</summary>
        [Column("password_hash")]
        [JsonIgnore]
        public string? PasswordHash { get; set; }

        [Column("empresa_id")]
        public int EmpresaId { get; set; }

        [Column("rol_id")]
        public int RolId { get; set; }

        // Navegación
        [ForeignKey("EmpresaId")]
        public Empresa? Empresa { get; set; }

        [ForeignKey("RolId")]
        public Catalogo.Rol? Rol { get; set; }

        public ICollection<Instalacion> InstalacionesTecnico { get; set; } = [];
        public ICollection<Mantenimiento> MantenimientosTecnico { get; set; } = [];
        public ICollection<Mantenimiento> MantenimientosSupervisor { get; set; } = [];
        public ICollection<Mantenimiento> MantenimientosEncargado { get; set; } = [];
        public ICollection<Contrato> ContratosEncargado { get; set; } = [];
        public ICollection<VisitaTecnica> VisitasTecnicas { get; set; } = [];
    }
}
