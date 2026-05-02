using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sismeing.Service.EntitiesDTO.Operaciones
{
    public class UsuarioDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = null!;
        public string Apellido { get; set; } = null!;
        public string Cedula { get; set; } = null!;
        public string CorreoElectronico { get; set; } = null!;
        public string? Telefono { get; set; }
        public int EmpresaId { get; set; }
        public int RolId { get; set; }
        public bool Verificado { get; set; }
    }
}
