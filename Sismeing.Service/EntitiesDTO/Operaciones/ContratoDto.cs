using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sismeing.Service.EntitiesDTO.Catalogo
{
    public class ContratoDto
    {
        public int Id { get; set; }
        public string NombreProyecto { get; set; } = null!;
        public int EmpresaId { get; set; }
        public int DireccionId { get; set; }
        public int EncargadoId { get; set; }
        public int TipoTrabajoId { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
    }
}
