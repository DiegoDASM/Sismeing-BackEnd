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

        // Número de informe automático y secuencial por año: VT-2026-0001.
        // La visita evalúa un prospecto (puede no tener empresa registrada), por
        // eso la secuencia es global por año y no por empresa como en los demás
        // informes.
        private async Task<string> SiguienteNumeroInformeAsync()
        {
            var anio = DateTime.UtcNow.Year;
            var prefijo = $"VT-{anio}-";

            var ultimo = await _context.VisitasTecnicas
                .Where(v => v.NumeroInforme != null && v.NumeroInforme.StartsWith(prefijo))
                .Select(v => v.NumeroInforme!)
                .ToListAsync();

            var siguiente = ultimo
                .Select(n => int.TryParse(n[prefijo.Length..], out var x) ? x : 0)
                .DefaultIfEmpty(0)
                .Max() + 1;

            return $"{prefijo}{siguiente:D4}";
        }

        public async Task<Visita_Tecnica> CreateAsync(Visita_Tecnica item, string usuarioRegistro)
        {
            item.FechaVisita = EntityUpdateHelper.AsegurarUtc(item.FechaVisita);
            item.Activo = true;
            // Número de informe automático (si no vino uno manual).
            if (string.IsNullOrWhiteSpace(item.NumeroInforme))
                item.NumeroInforme = await SiguienteNumeroInformeAsync();
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
            EntityUpdateHelper.PreservarSiVacio(entry, "NumeroInforme");
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

        // Reactiva un registro previamente desactivado (activo = true).
        public async Task<bool> ReactivarAsync(int id, string usuario)
        {
            var item = await _context.VisitasTecnicas.FindAsync(id);
            if (item == null) return false;

            item.Activo = true;
            item.UsuarioModificacion = usuario;
            item.FechaModificacion = DateTime.UtcNow;
            item.IpModificacion = _auditoriaService.ObtenerIp();

            await _context.SaveChangesAsync();
            return true;
        }

    }
}
