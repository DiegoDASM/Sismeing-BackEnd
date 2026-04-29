using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sismeing.Service.EntitiesDTO.Catalogo
{
    public class DireccionEmpresaDto
    {
        public int Id { get; set; }
        public int EmpresaId { get; set; }
        public string Direccion { get; set; } = null!;
    }
}
