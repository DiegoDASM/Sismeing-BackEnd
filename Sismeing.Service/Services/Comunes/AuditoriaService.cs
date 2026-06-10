using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sismeing.Service.Interfaces.Comunes;
using Microsoft.AspNetCore.Http;

namespace Sismeing.Service.Services.Comunes
{
    public class AuditoriaService : IAuditoriaService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuditoriaService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string ObtenerIp()
        {
            var ip = _httpContextAccessor.HttpContext?
                .Connection?
                .RemoteIpAddress;

            if (ip == null)
                return "127.0.0.1";

            // Si es la dirección local (loopback) de IPv6 (::1) o IPv4 (127.0.0.1)
            if (System.Net.IPAddress.IsLoopback(ip))
                return "127.0.0.1";

            return ip.MapToIPv4().ToString();
        }

    }
}
