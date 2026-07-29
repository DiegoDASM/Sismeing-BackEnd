using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sismeing.Domain.Entities.Catalogo;

namespace Sismeing.Domain.Entities.Operaciones
{
    [Table("usuario", Schema = "public")]
    public class Usuario : AuditCatProperties
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }

        [Column("nombre")]
        public string Nombre { get; set; } = null!;

        [Column("apellido")]
        public string Apellido { get; set; } = null!;

        [Column("cedula")]
        public string Cedula { get; set; } = null!;

        [Column("correo_electronico")]
        public string CorreoElectronico { get; set; } = null!;

        [Column("telefono")]
        public string? Telefono { get; set; }

        [Column("verificado")]
        public bool Verificado { get; set; }

        [Column("empresa_id")]
        public int EmpresaId { get; set; }

        [Column("rol_id")]
        public int RolId { get; set; }

        // Nunca debe salir en las respuestas JSON (se incluye vía navegaciones
        // Tecnico/Colaboradores/etc.). Sigue disponible para el login en el backend.
        [System.Text.Json.Serialization.JsonIgnore]
        [Column("Contrasena")]
        public string Contrasena { get; set; } = null!;

        // Token de invitación de encargado: presente mientras el usuario no ha
        // completado su registro. Se limpia (null) al completarlo.
        [System.Text.Json.Serialization.JsonIgnore]
        [Column("invitacion_token")]
        public string? InvitacionToken { get; set; }

        [ForeignKey("EmpresaId")]
        public virtual Empresa? Empresa { get; set; }
        [ForeignKey("RolId")]
        public virtual Rol? Rol { get; set; }
    }
}
