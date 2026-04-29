using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sismeing.Service.EntitiesDTO.Catalogo
{
    public class MantenimientoDto
    {
        public int Id { get; set; }
        public int InstalacionId { get; set; }
        public int TecnicoId { get; set; }
        public string? ObservacionInicial { get; set; }
        public string? ObservacionesFinales { get; set; }
        public bool RequiereRepuestos { get; set; }
        public int TipoMantenimientoId { get; set; }
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public DateTime? FechaProximo { get; set; }
        public int EstadoId { get; set; }
        public int? SupervisorId { get; set; }
        public int? EncargadoId { get; set; }
        public string? NumeroInforme { get; set; }
    }
}
