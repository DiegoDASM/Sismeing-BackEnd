using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sismeing.Domain.Entities.Operaciones
{
    // Colaborador (tecnico adicional) de un mantenimiento. El responsable sigue en
    // Mantenimiento.TecnicoId; aqui van los demas tecnicos que participaron.
    [Table("mantenimiento_tecnico", Schema = "public")]
    public class Mantenimiento_Tecnico
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }
        [Column("mantenimiento_id")]
        public int MantenimientoId { get; set; }
        [Column("usuario_id")]
        public int UsuarioId { get; set; }

        [ForeignKey("MantenimientoId")]
        public virtual Mantenimiento? Mantenimiento { get; set; }
        [ForeignKey("UsuarioId")]
        public virtual Usuario? Usuario { get; set; }
    }
}
