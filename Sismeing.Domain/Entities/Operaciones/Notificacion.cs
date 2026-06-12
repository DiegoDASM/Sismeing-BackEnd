using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sismeing.Domain.Entities.Operaciones
{
    [Table("notificacion", Schema = "public")]
    public class Notificacion : AuditCatProperties
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }

        [Column("usuario_id")]
        public int UsuarioId { get; set; }

        [Column("titulo")]
        public string Titulo { get; set; } = string.Empty;

        [Column("mensaje")]
        public string Mensaje { get; set; } = string.Empty;

        // 'aprobado' | 'pendiente' | 'programado' | 'completado' | 'info'
        [Column("tipo")]
        public string Tipo { get; set; } = "info";

        // Módulo que generó la notificación: 'mantenimiento' | 'instalacion' | 'visita_tecnica' | ...
        [Column("origen")]
        public string? Origen { get; set; }

        [Column("referencia_id")]
        public int? ReferenciaId { get; set; }

        [Column("leida")]
        public bool Leida { get; set; }

        [Column("fecha_leida")]
        public DateTime? FechaLeida { get; set; }

        [ForeignKey("UsuarioId")]
        public virtual Usuario? Usuario { get; set; }
    }
}
