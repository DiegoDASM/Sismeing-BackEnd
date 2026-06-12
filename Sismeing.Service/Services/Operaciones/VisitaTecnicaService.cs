using Microsoft.EntityFrameworkCore;
using Sismeing.Domain.Entities.Operaciones;
using Sismeing.Infrestructura.Persistence;
using Sismeing.Service.Interfaces.Comunes;
using Sismeing.Service.Interfaces.Operaciones;
using Sismeing.Service.Services.Comunes;

namespace Sismeing.Service.Services.Operaciones
{
    public class VisitaTecnicaService : IVisita_TecnicaService
    {
        private readonly SupaBaseDBcontext _context;
        private readonly IAuditoriaService _auditoriaService;
        private readonly INotificacionService _notificacionService;

        public VisitaTecnicaService(SupaBaseDBcontext context, IAuditoriaService auditoriaService, INotificacionService notificacionService)
        {
            _context = context;
            _auditoriaService = auditoriaService;
            _notificacionService = notificacionService;
        }

        public async Task<IEnumerable<Visita_Tecnica>> GetAllAsync()
        {
            return await _context.VisitasTecnicas
                .Include(v => v.Empresa)
                .Include(v => v.Tecnico)
                .Include(v => v.TipoTrabajo)
                .ToListAsync();
        }

        public async Task<Visita_Tecnica?> GetByIdAsync(int id)
        {
            return await _context.VisitasTecnicas
                .Include(v => v.Empresa)
                .Include(v => v.Tecnico)
                .Include(v => v.TipoTrabajo)
                .FirstOrDefaultAsync(v => v.Id == id);
        }

        public async Task<Visita_Tecnica> CreateAsync(Visita_Tecnica item, string usuarioRegistro)
        {
            item.FechaVisita = EntityUpdateHelper.AsegurarUtc(item.FechaVisita);
            item.Activo = true;
            item.UsuarioRegistro = usuarioRegistro;
            item.FechaRegistro = DateTime.UtcNow;
            item.IpRegistro = _auditoriaService.ObtenerIp();

            _context.VisitasTecnicas.Add(item);
            await _context.SaveChangesAsync();

            // Notificación in-app al técnico asignado
            try
            {
                var empresa = await _context.Empresas.FindAsync(item.EmpresaId);
                await _notificacionService.CreateAsync(new Notificacion
                {
                    UsuarioId = item.TecnicoId,
                    Titulo = "Visita Técnica Registrada",
                    Mensaje = $"Se registró una visita técnica a {empresa?.Nombre ?? "una empresa"} para el {item.FechaVisita:dd/MM/yyyy}.",
                    Tipo = "programado",
                    Origen = "visita_tecnica",
                    ReferenciaId = item.Id,
                }, usuarioRegistro);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creando notificación de visita técnica: {ex.GetBaseException().Message}");
            }

            return item;
        }

        public async Task<bool> UpdateAsync(int id, Visita_Tecnica item, string usuarioModificacion)
        {
            var existingItem = await _context.VisitasTecnicas.FindAsync(id);
            if (existingItem == null) return false;

            item.FechaVisita = EntityUpdateHelper.AsegurarUtc(item.FechaVisita);
            var entry = _context.Entry(existingItem);
            entry.CurrentValues.SetValues(item);
            EntityUpdateHelper.PreservarCamposRegistro(entry);
            existingItem.UsuarioModificacion = usuarioModificacion;
            existingItem.FechaModificacion = DateTime.UtcNow;
            existingItem.IpModificacion = _auditoriaService.ObtenerIp();

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id, string usuarioEliminacion)
        {
            var existingItem = await _context.VisitasTecnicas.FindAsync(id);
            if (existingItem == null) return false;

            existingItem.Activo = false;
            existingItem.UsuarioEliminacion = usuarioEliminacion;
            existingItem.FechaEliminacion = DateTime.UtcNow;
            existingItem.IpEliminacion = _auditoriaService.ObtenerIp();

            await _context.SaveChangesAsync();
            return true;
        }
    }
}
