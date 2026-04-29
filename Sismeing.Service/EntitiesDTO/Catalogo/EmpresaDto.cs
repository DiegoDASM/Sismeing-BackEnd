using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sismeing.Service.EntitiesDTO.Catalogo
{
    public class EmpresaDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = null!;
        public string RazonSocial { get; set; } = null!;
        public string? Telefono { get; set; }
        public string? CorreoElectronico { get; set; }
        public string? Logo { get; set; }
        public bool Activo { get; set; }
    }
}
