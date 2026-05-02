using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sismeing.Service.EntitiesDTO.Catalogo
{
    public class AreaEmpresaDto
    {
        public int Id { get; set; }
        public string NombreArea { get; set; } = null!;
        public int EmpresaId { get; set; }
        public string? NombreEmpresa { get; set; }
    }
}
