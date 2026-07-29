using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sismeing.Domain.Entities.Operaciones
{
    // Colaborador (tecnico adicional) de una instalacion. El responsable sigue en
    // Instalacion.TecnicoId; aqui van los demas tecnicos que participaron.
    [Table("instalacion_tecnico", Schema = "public")]
    public class Instalacion_Tecnico
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }
        [Column("instalacion_id")]
        public int InstalacionId { get; set; }
        [Column("usuario_id")]
        public int UsuarioId { get; set; }

        [ForeignKey("InstalacionId")]
        public virtual Instalacion? Instalacion { get; set; }
        [ForeignKey("UsuarioId")]
        public virtual Usuario? Usuario { get; set; }
    }
}
