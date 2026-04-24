using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sismeing.Domain.Entities.DTOs;
using Sismeing.Domain.Entities.Operaciones;
using Sismeing.Infrestructura.Persistence;

namespace Sismeing.API.Controllers.Operaciones
{
    [Authorize]
    [ApiController]
    [Route("api/usuario")]
    public class UsuarioController : ControllerBase
    {
        private readonly SupaBaseDBcontext _context;
        public UsuarioController(SupaBaseDBcontext context) => _context = context;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UsuarioDto>>> GetAll()
        {
            var usuarios = await _context.Usuarios
                .Include(u => u.Rol)
                .Include(u => u.Empresa)
                .Where(u => u.Activo)
                .Select(u => new UsuarioDto
                {
                    Id = u.Id,
                    Nombre = u.Nombre,
                    Apellido = u.Apellido,
                    CorreoElectronico = u.CorreoElectronico,
                    Telefono = u.Telefono,
                    NombreRol = u.Rol != null ? u.Rol.NombreRol : string.Empty,
                    RolId = u.RolId,
                    EmpresaId = u.EmpresaId,
                    NombreEmpresa = u.Empresa != null ? u.Empresa.Nombre : string.Empty,
                    Verificado = u.Verificado
                }).ToListAsync();
            return Ok(usuarios);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<UsuarioDto>> GetById(int id)
        {
            var u = await _context.Usuarios
                .Include(u => u.Rol)
                .Include(u => u.Empresa)
                .FirstOrDefaultAsync(u => u.Id == id);
            if (u == null) return NotFound();
            return Ok(new UsuarioDto
            {
                Id = u.Id, Nombre = u.Nombre, Apellido = u.Apellido,
                CorreoElectronico = u.CorreoElectronico, Telefono = u.Telefono,
                NombreRol = u.Rol?.NombreRol ?? string.Empty, RolId = u.RolId,
                EmpresaId = u.EmpresaId,
                NombreEmpresa = u.Empresa?.Nombre ?? string.Empty,
                Verificado = u.Verificado
            });
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] RegisterRequestDto request)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null) return NotFound();
            usuario.Nombre = request.Nombre;
            usuario.Apellido = request.Apellido;
            usuario.Telefono = request.Telefono;
            usuario.RolId = request.RolId;
            usuario.EmpresaId = request.EmpresaId;
            if (!string.IsNullOrEmpty(request.Password))
                usuario.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
            usuario.FechaModificacion = DateTime.UtcNow;
            usuario.UsuarioModificacion = HttpContext.Items["UserEmail"]?.ToString();
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null) return NotFound();
            usuario.Activo = false;
            usuario.FechaEliminacion = DateTime.UtcNow;
            usuario.UsuarioEliminacion = HttpContext.Items["UserEmail"]?.ToString();
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPatch("{id:int}/verificar")]
        public async Task<IActionResult> Verificar(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null) return NotFound();
            usuario.Verificado = true;
            usuario.FechaModificacion = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
