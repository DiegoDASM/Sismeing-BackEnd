using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sismeing.Service.EntitiesDTO.Catalogo
{
    public class MedicionDto
    {
        public int Id { get; set; }
        public decimal? Voltaje { get; set; }
        public decimal? Frecuencia { get; set; }
        public decimal? PresionSuccionPsi { get; set; }
        public decimal? PresionDescargaPsi { get; set; }
        public int EquipoId { get; set; }
        public int AreaId { get; set; }
        public bool Inicial { get; set; }
    }
}
