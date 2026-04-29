using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sismeing.Service.EntitiesDTO.Catalogo
{
    public class FotoInstalacionDto
    {
        public int Id { get; set; }
        public int InstalacionId { get; set; }
        public string Url { get; set; } = null!;
        public string? Tipo { get; set; }
    }
}
