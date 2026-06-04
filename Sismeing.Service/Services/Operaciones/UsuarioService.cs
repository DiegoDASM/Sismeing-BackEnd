using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Sismeing.Domain.Entities.Operaciones;
using Sismeing.Infrestructura.Persistence;
using Sismeing.Service.Interfaces.Operaciones;

namespace Sismeing.Service.Services.Operaciones
{
    public class UsuarioService : IUsuarioService
    {
        private readonly SupaBaseDBcontext _context;
        private readonly IPasswordHasher<Usuario> _passwordHasher;

        public UsuarioService(SupaBaseDBcontext context, IPasswordHasher<Usuario> passwordHasher)
        {
            _context = context;
            _passwordHasher = passwordHasher;
        }

        public async Task<IEnumerable<Usuario>> GetAllAsync()
        {
            return await _context.Usuarios.Where(u => u.Activo).ToListAsync();
        }

        public async Task<Usuario?> GetByIdAsync(int id)
        {
            return await _context.Usuarios.FindAsync(id);
        }

        public async Task<Usuario> CreateAsync(Usuario item, string usuarioRegistro)
        {

            if (!string.IsNullOrEmpty(item.Contrasena))
            {
                item.Contrasena = _passwordHasher.HashPassword(item, item.Contrasena);
            }

            item.UsuarioRegistro = usuarioRegistro;
            item.FechaRegistro = DateTime.UtcNow;
            item.Activo = true;

            _context.Usuarios.Add(item);
            await _context.SaveChangesAsync();

            return item;
        }

        public async Task<bool> UpdateAsync(int id, Usuario item, string usuarioModificacion)
        {
            var existingItem = await _context.Usuarios.FindAsync(id);
            if (existingItem == null) return false;

            _context.Entry(existingItem).CurrentValues.SetValues(item);
            existingItem.UsuarioModificacion = usuarioModificacion;
            existingItem.FechaModificacion = DateTime.UtcNow;
            
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id, string usuarioEliminacion)
        {
            var existingItem = await _context.Usuarios.FindAsync(id);
            if (existingItem == null) return false;

            existingItem.Activo = false;
            existingItem.UsuarioEliminacion = usuarioEliminacion;
            existingItem.FechaEliminacion = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdatePerfilAsync(int id, string nombre, string apellido, string? telefono, string userEmail)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null) return false;

            usuario.Nombre = nombre;
            usuario.Apellido = apellido;
            usuario.Telefono = telefono;
            usuario.UsuarioModificacion = userEmail;
            usuario.FechaModificacion = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

    }
}