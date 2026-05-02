using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sismeing.Service.EntitiesDTO.Catalogo
{
    public class VisitaTecnicaDto
    {
        public int Id { get; set; }
        public int EmpresaId { get; set; }
        public int TecnicoId { get; set; }
        public int TipoTrabajoId { get; set; }
        public DateTime FechaVisita { get; set; }
        public string? DescripcionVisita { get; set; }
        public string? NumeroInforme { get; set; }
    }
}
