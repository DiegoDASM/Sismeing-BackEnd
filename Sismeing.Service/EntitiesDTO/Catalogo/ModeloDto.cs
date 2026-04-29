using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sismeing.Service.EntitiesDTO.Catalogo
{
    public class ModeloDto
    {
        public int Id { get; set; }
        public string NombreModelo { get; set; } = null!;
        public string? Capacidad { get; set; }
        public string? Potencia { get; set; }
        public short? AnioFabricacion { get; set; }
    }
}
