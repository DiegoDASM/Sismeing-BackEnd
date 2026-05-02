using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sismeing.Service.EntitiesDTO.Operaciones
{
    public class InstalacionDto
    {
        public int Id { get; set; }
        public int EquipoId { get; set; }
        public int AreaId { get; set; }
        public int TecnicoId { get; set; }
        public string? OrdenTrabajo { get; set; }
        public decimal? HorasTrabajadas { get; set; }
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public int EstadoId { get; set; }
        public string? NumeroInforme { get; set; }
    }
}
