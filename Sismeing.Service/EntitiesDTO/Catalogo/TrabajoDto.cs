using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sismeing.Service.EntitiesDTO.Catalogo
{
    public class TrabajoDto
    {
        public int Id { get; set; }
        public string NombreTrabajo { get; set; } = null!;
        public string? Descripcion { get; set; }
    }
}
