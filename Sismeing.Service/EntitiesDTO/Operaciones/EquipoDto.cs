using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sismeing.Service.EntitiesDTO.Catalogo
{
    public class EquipoDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = null!;
        public int MarcaId { get; set; }
        public int TipoId { get; set; }
        public int ModeloId { get; set; }
        public string? Codigo { get; set; }
        public string? NumeroSerie { get; set; }
        public int? ProyectoId { get; set; }
    }
}
