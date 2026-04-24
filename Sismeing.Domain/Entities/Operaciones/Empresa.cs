using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sismeing.Domain.Entities.Operaciones
{
    [Table("empresa", Schema = "public")]
    public class Empresa : AuditProperties
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("nombre")]
        [StringLength(255)]
        public string Nombre { get; set; } = string.Empty;

        [Required]
        [Column("razon_social")]
        [StringLength(255)]
        public string RazonSocial { get; set; } = string.Empty;

        [Column("telefono")]
        [StringLength(50)]
        public string? Telefono { get; set; }

        [Column("correo_electronico")]
        [StringLength(255)]
        [EmailAddress]
        public string? CorreoElectronico { get; set; }

        [Column("logo")]
        public string? Logo { get; set; }

        // Navegación
        public ICollection<Usuario> Usuarios { get; set; } = [];
        public ICollection<AreaEmpresa> AreasEmpresa { get; set; } = [];
        public ICollection<DireccionEmpresa> DireccionesEmpresa { get; set; } = [];
        public ICollection<Contrato> Contratos { get; set; } = [];
        public ICollection<VisitaTecnica> VisitasTecnicas { get; set; } = [];
    }
}
