using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sismeing.Service.EntitiesDTO
{
    public class LoginDto
    {
        public string CorreoElectronico { get; set; } = null!;
        public string Contrasena { get; set; } = null!;
    }
}
