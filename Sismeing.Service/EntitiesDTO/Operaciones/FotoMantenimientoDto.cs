using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sismeing.Service.EntitiesDTO.Operaciones
{
    public class FotoMantenimientoDto
    {
        public int Id { get; set; }
        public int MantenimientoId { get; set; }
        public string Url { get; set; } = null!;
        public string? Tipo { get; set; }
    }
}
