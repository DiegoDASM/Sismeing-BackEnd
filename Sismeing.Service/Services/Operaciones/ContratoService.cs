using Microsoft.EntityFrameworkCore;
using Sismeing.Domain.Entities.Operaciones;
using Sismeing.Infrestructura.Persistence;
using Sismeing.Service.Interfaces.Comunes;
using Sismeing.Service.Interfaces.Operaciones;
using Sismeing.Service.Services.Comunes;

namespace Sismeing.Service.Services.Operaciones
{
    public class ContratoService : IContratoService
    {
        private readonly SupaBaseDBcontext _context;
        private readonly IAuditoriaService _auditoriaService;


        public ContratoService(SupaBaseDBcontext context, IAuditoriaService auditoriaService)
        {
            _context = context;
            _auditoriaService = auditoriaService;
        }

        public async Task<IEnumerable<Contrato>> GetAllAsync()
        {
            return await _context.Contratos
                .Include(c => c.Empresa)
                .Include(c => c.TipoTrabajo)
                .Include(c => c.TiposTrabajo).ThenInclude(t => t.TipoTrabajo)
                .Include(c => c.Encargado)
                .ToListAsync();
        }

        public async Task<Contrato?> GetByIdAsync(int id)
        {
            return await _context.Contratos
                .Include(c => c.Empresa)
                .Include(c => c.TipoTrabajo)
                .Include(c => c.TiposTrabajo).ThenInclude(t => t.TipoTrabajo)
                .Include(c => c.Encargado)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        // Reemplaza el conjunto de tipos de trabajo del contrato (Instalacion/Mantenimiento).
        private async Task SincronizarTiposAsync(int contratoId, List<int>? ids)
        {
            if (ids == null) return;
            var deseados = ids.Distinct().ToList();
            var actuales = await _context.ContratoTiposTrabajo.Where(t => t.ContratoId == contratoId).ToListAsync();

            var aEliminar = actuales.Where(a => !deseados.Contains(a.TipoTrabajoId)).ToList();
            if (aEliminar.Count > 0) _context.ContratoTiposTrabajo.RemoveRange(aEliminar);

            var existentes = actuales.Select(a => a.TipoTrabajoId).ToHashSet();
            foreach (var tid in deseados.Where(tid => !existentes.Contains(tid)))
                _context.ContratoTiposTrabajo.Add(new Contrato_TipoTrabajo { ContratoId = contratoId, TipoTrabajoId = tid });

            await _context.SaveChangesAsync();
        }

        private static void NormalizarFechas(Contrato item)
        {
            item.FechaInicio = EntityUpdateHelper.AsegurarUtc(item.FechaInicio);
            item.FechaFin = EntityUpdateHelper.AsegurarUtc(item.FechaFin);
        }

        // Un contrato debe cubrir al menos un tipo de trabajo (Instalacion/Mantenimiento).
        private static void ValidarTipos(Contrato item)
        {
            var hayLista = item.TipoTrabajoIds != null && item.TipoTrabajoIds.Count > 0;
            if (!hayLista && item.TipoTrabajoId <= 0)
                throw new InvalidOperationException("Seleccione al menos un tipo de trabajo.");
        }

        public async Task<Contrato> CreateAsync(Contrato item, string usuarioRegistro)
        {
            ValidarTipos(item);
            NormalizarFechas(item);
            // Tipo primario = primero de la lista (compatibilidad con tipo_trabajo_id).
            if (item.TipoTrabajoIds != null && item.TipoTrabajoIds.Count > 0)
                item.TipoTrabajoId = item.TipoTrabajoIds[0];
            item.Activo = true;
            item.UsuarioRegistro = usuarioRegistro;
            item.FechaRegistro = DateTime.UtcNow;
            item.IpRegistro = _auditoriaService.ObtenerIp();

            _context.Contratos.Add(item);
            await _context.SaveChangesAsync();

            await SincronizarTiposAsync(item.Id, item.TipoTrabajoIds);

            return item;
        }

        public async Task<bool> UpdateAsync(int id, Contrato item, string usuarioModificacion)
        {
            var existingItem = await _context.Contratos.FindAsync(id);
            if (existingItem == null) return false;

            ValidarTipos(item);
            NormalizarFechas(item);
            if (item.TipoTrabajoIds != null && item.TipoTrabajoIds.Count > 0)
                item.TipoTrabajoId = item.TipoTrabajoIds[0];
            var entry = _context.Entry(existingItem);
            entry.CurrentValues.SetValues(item);
            EntityUpdateHelper.PreservarCamposRegistro(entry);
            existingItem.UsuarioModificacion = usuarioModificacion;
            existingItem.FechaModificacion = DateTime.UtcNow;
            existingItem.IpModificacion = _auditoriaService.ObtenerIp();

            await _context.SaveChangesAsync();

            await SincronizarTiposAsync(existingItem.Id, item.TipoTrabajoIds);
            return true;
        }

        public async Task<bool> DeleteAsync(int id, string usuarioEliminacion)
        {
            var existingItem = await _context.Contratos.FindAsync(id);
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
            var item = await _context.Contratos.FindAsync(id);
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