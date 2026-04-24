using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sismeing.Domain.Entities.Operaciones
{
    [Table("equipo", Schema = "public")]
    public class Equipo : AuditProperties
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("nombre")]
        [StringLength(255)]
        public string Nombre { get; set; } = string.Empty;

        [Column("marca_id")]
        public int MarcaId { get; set; }

        [Column("tipo_id")]
        public int TipoId { get; set; }

        [Column("modelo_id")]
        public int ModeloId { get; set; }

        [Column("codigo")]
        [StringLength(100)]
        public string? Codigo { get; set; }

        [Column("numero_serie")]
        [StringLength(100)]
        public string? NumeroSerie { get; set; }

        [Column("proyecto_id")]
        public int? ProyectoId { get; set; }

        // Navegación
        [ForeignKey("MarcaId")]
        public Catalogo.Marca? Marca { get; set; }

        [ForeignKey("TipoId")]
        public Catalogo.TipoEquipo? Tipo { get; set; }

        [ForeignKey("ModeloId")]
        public Catalogo.Modelo? Modelo { get; set; }

        [ForeignKey("ProyectoId")]
        public Contrato? Proyecto { get; set; }

        public ICollection<Instalacion> Instalaciones { get; set; } = [];
        public ICollection<Medicion> Mediciones { get; set; } = [];
    }
}
