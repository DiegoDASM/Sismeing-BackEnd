using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sismeing.Infrestructura.Persistence;

namespace Sismeing.API.Controllers
{
    [ApiController]
    [Route("api/health")]
    public class HealthController : ControllerBase
    {
        private readonly SupaBaseDBcontext _context;

        public HealthController(SupaBaseDBcontext context)
        {
            _context = context;
        }

        /// <summary>Verifica que la API está en línea.</summary>
        [AllowAnonymous]
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new
            {
                estado = "OK",
                version = "1.0.0",
                timestamp = DateTime.UtcNow
            });
        }

        /// <summary>Verifica la conexión activa con la base de datos Supabase.</summary>
        [AllowAnonymous]
        [HttpGet("db")]
        public async Task<IActionResult> CheckDatabase()
        {
            try
            {
                var puedoConectar = await _context.Database.CanConnectAsync();

                if (!puedoConectar)
                    return StatusCode(503, new
                    {
                        estado = "ERROR",
                        mensaje = "No se puede conectar a la base de datos.",
                        timestamp = DateTime.UtcNow
                    });

                // Prueba real: cuenta un registro simple
                var totalEmpresas = await _context.Empresas.CountAsync();

                return Ok(new
                {
                    estado = "OK",
                    mensaje = "Conexión a Supabase PostgreSQL activa.",
                    empresasRegistradas = totalEmpresas,
                    timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                return StatusCode(503, new
                {
                    estado = "ERROR",
                    mensaje = "Error al conectar con la base de datos.",
                    detalle = ex.Message,
                    timestamp = DateTime.UtcNow
                });
            }
        }
    }
}
