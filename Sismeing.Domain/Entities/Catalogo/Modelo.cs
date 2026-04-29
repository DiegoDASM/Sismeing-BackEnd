using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sismeing.Domain.Entities.Catalogo
{
    [Table(nameof(Modelo), Schema = "public")]
    public class Modelo : AuditCatProperties
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string NombreModelo { get; set; } = null!;
        public string? Capacidad { get; set; }
        public string? Potencia { get; set; }
        public short? AnioFabricacion { get; set; }
    }
}
