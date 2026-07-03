using Microsoft.EntityFrameworkCore;
using Sismeing.Domain.Entities.Operaciones;
using Sismeing.Domain.Models.Informes;
using Sismeing.Infrestructura.Persistence;
using Sismeing.Service.Interfaces.Comunes;
using Sismeing.Service.Interfaces.Operaciones;

namespace Sismeing.Service.Services.Operaciones
{
    public class ReporteService : IReporteService
    {
        private readonly SupaBaseDBcontext _context;
        private readonly IEmailPreviewService _render;

        public ReporteService(SupaBaseDBcontext context, IEmailPreviewService render)
        {
            _context = context;
            _render = render;
        }

        // ── Helpers ────────────────────────────────────────────────────────────
        private static string Num(string? numeroInforme, int id) =>
            string.IsNullOrWhiteSpace(numeroInforme) ? $"#{id}" : numeroInforme!;

        private static string? NombreCompleto(Usuario? u) =>
            u == null ? null : $"{u.Nombre} {u.Apellido}".Trim();

        private Task<List<string>> UrlsAsync(IQueryable<string> q) => q.ToListAsync();

        private static List<InformeFotoGrupo> Grupos(List<string> inicial, List<string> final) => new()
        {
            new InformeFotoGrupo { Titulo = "Imagen Inicial", Urls = inicial },
            new InformeFotoGrupo { Titulo = "Imagen Final", Urls = final },
        };

        private async Task<List<InformeFotoGrupo>> FotosInstalacionAsync(int id)
        {
            var ini = await _context.FotosInstalacion.Where(f => f.InstalacionId == id && f.Activo && f.Tipo == "inicial")
                .OrderBy(f => f.FechaRegistro).Select(f => f.Url).ToListAsync();
            var fin = await _context.FotosInstalacion.Where(f => f.InstalacionId == id && f.Activo && f.Tipo == "final")
                .OrderBy(f => f.FechaRegistro).Select(f => f.Url).ToListAsync();
            return Grupos(ini, fin);
        }

        private async Task<List<InformeFotoGrupo>> FotosMantenimientoAsync(int id)
        {
            var ini = await _context.FotosMantenimiento.Where(f => f.MantenimientoId == id && f.Activo && f.Tipo == "inicial")
                .OrderBy(f => f.FechaRegistro).Select(f => f.Url).ToListAsync();
            var fin = await _context.FotosMantenimiento.Where(f => f.MantenimientoId == id && f.Activo && f.Tipo == "final")
                .OrderBy(f => f.FechaRegistro).Select(f => f.Url).ToListAsync();
            return Grupos(ini, fin);
        }

        private async Task<List<InformeFotoGrupo>> FotosVisitaAsync(int id)
        {
            var ini = await _context.FotosVisitaTecnica.Where(f => f.VisitaTecnicaId == id && f.Activo && f.Tipo == "inicial")
                .OrderBy(f => f.FechaRegistro).Select(f => f.Url).ToListAsync();
            var fin = await _context.FotosVisitaTecnica.Where(f => f.VisitaTecnicaId == id && f.Activo && f.Tipo == "final")
                .OrderBy(f => f.FechaRegistro).Select(f => f.Url).ToListAsync();
            return Grupos(ini, fin);
        }

        /// <summary>
        /// Construye el bloque de mediciones (inicial/final) para un informe y equipo.
        /// Se vincula por informe_id == idInforme AND equipo_id == equipoId (acordado con el cliente).
        /// Devuelve null si no hay mediciones registradas.
        /// </summary>
        private async Task<InformeMediciones?> BuildMedicionesAsync(int idInforme, int? equipoId)
        {
            if (equipoId is null) return null;

            var mediciones = await _context.Mediciones
                .Where(m => m.InformeId == idInforme && m.EquipoId == equipoId && m.Activo)
                .ToListAsync();
            if (mediciones.Count == 0) return null;

            var ini = mediciones.FirstOrDefault(m => m.Inicial);
            var fin = mediciones.FirstOrDefault(m => !m.Inicial);
            if (ini is null && fin is null) return null;

            var b = new InformeMediciones();
            b.Add("Voltaje (V)", ini?.Voltaje, fin?.Voltaje);
            b.Add("Frecuencia (Hz)", ini?.Frecuencia, fin?.Frecuencia);
            b.Add("Amp. Evap. / Ventilador (RLA)", ini?.AmpEvaporadorVentiladorRla, fin?.AmpEvaporadorVentiladorRla);
            b.Add("Amp. Motor Condensadora (RLA)", ini?.AmpMotorCondensadoraRla, fin?.AmpMotorCondensadoraRla);
            b.Add("Amp. Compresor (RLA)", ini?.AmpCompresorRla, fin?.AmpCompresorRla);
            b.Add("Presión Succión (psi)", ini?.PresionSuccionPsi, fin?.PresionSuccionPsi);
            b.Add("Presión Descarga (psi)", ini?.PresionDescargaPsi, fin?.PresionDescargaPsi);
            b.Add("Temp. Evaporador (°C)", ini?.TempInicialFinalEvapC, fin?.TempInicialFinalEvapC);
            b.Add("Temp. Condensador (°C)", ini?.TempInicialFinalCondC, fin?.TempInicialFinalCondC);
            b.Add("Temp. Ingreso / Salida Agua (°C)", ini?.TempIngresoSalidaAguaC, fin?.TempIngresoSalidaAguaC);
            b.Add("Temperatura Programada (°C)", ini?.TemperaturaProgramadaC, fin?.TemperaturaProgramadaC);
            b.Add("Humedad Relativa Progr. (%)", ini?.HumedadRelativaProgPct, fin?.HumedadRelativaProgPct);

            return b.Filas.Count == 0 ? null : b;
        }

        // ════════════════════════════════════════════════════════════════════════
        // INSTALACIÓN
        // ════════════════════════════════════════════════════════════════════════
        public async Task<string?> InstalacionDatosAsync(int id)
        {
            var i = await _context.Instalaciones
                .Include(x => x.Equipo).ThenInclude(e => e!.Marca)
                .Include(x => x.Equipo).ThenInclude(e => e!.Modelo)
                .Include(x => x.Equipo).ThenInclude(e => e!.TipoEquipo)
                .Include(x => x.Area).ThenInclude(a => a!.Empresa)
                .Include(x => x.Area).ThenInclude(a => a!.DireccionEmpresa)
                .Include(x => x.Tecnico)
                .Include(x => x.Estado)
                .FirstOrDefaultAsync(x => x.Id == id);
            if (i == null) return null;

            var m = new InformeDatosModel
            {
                Titulo = "INFORME DE INSTALACIÓN",
                NumeroInforme = Num(i.NumeroInforme, i.Id),
                Estado = i.Estado?.NombreEstado
            };

            m.Secciones.Add(new InformeSeccion { Titulo = "Datos del Servicio" }
                .Add("Empresa", i.Area?.Empresa?.Nombre)
                .Add("Orden de Trabajo", i.OrdenTrabajo)
                .Add("Responsable", NombreCompleto(i.Tecnico))
                .Add("Dirección", i.Area?.DireccionEmpresa?.Direccion)
                .Add("Fecha de instalación", i.FechaInicio)
                .Add("Fecha de finalización", i.FechaFin)
                .Add("Horas de trabajo", i.HorasTrabajadas));

            m.Secciones.Add(new InformeSeccion { Titulo = "Datos del Equipo" }
                .Add("Equipo", i.Equipo?.Nombre)
                .Add("Tipo", i.Equipo?.TipoEquipo?.Nombre)
                .Add("No. de serie", i.Equipo?.NumeroSerie)
                .Add("Marca", i.Equipo?.Marca?.Nombre)
                .Add("Modelo", i.Equipo?.Modelo?.Nombre)
                .Add("Área", i.Area?.NombreArea)
                .Add("Capacidad", i.Equipo?.Modelo?.Capacidad)
                .Add("Código", i.Equipo?.Codigo)
                .Add("Potencia", i.Equipo?.Modelo?.Potencia));

            m.Mediciones = await BuildMedicionesAsync(i.Id, i.EquipoId);
            m.Fotos = await FotosInstalacionAsync(i.Id);
            m.SupervisorNombre = NombreCompleto(i.Tecnico);

            return await _render.RenderTemplateAsync("InformeDatos", m);
        }

        public async Task<string?> InstalacionFotograficoAsync(int id)
        {
            var i = await _context.Instalaciones
                .Include(x => x.Equipo)
                .Include(x => x.Tecnico)
                .FirstOrDefaultAsync(x => x.Id == id);
            if (i == null) return null;

            var m = new InformeFotograficoModel
            {
                Titulo = "INFORME FOTOGRÁFICO DE INSTALACIÓN",
                NumeroInforme = Num(i.NumeroInforme, i.Id),
                Descripcion = string.IsNullOrWhiteSpace(i.OrdenTrabajo)
                    ? i.Equipo?.Nombre
                    : $"Orden de trabajo {i.OrdenTrabajo}. Equipo: {i.Equipo?.Nombre}"
            };
            m.Grupos = await FotosInstalacionAsync(i.Id);

            m.SupervisorNombre = NombreCompleto(i.Tecnico);

            return await _render.RenderTemplateAsync("InformeFotografico", m);
        }

        // ════════════════════════════════════════════════════════════════════════
        // MANTENIMIENTO
        // ════════════════════════════════════════════════════════════════════════
        public async Task<string?> MantenimientoDatosAsync(int id)
        {
            var x = await _context.Mantenimientos
                .Include(m => m.Instalacion).ThenInclude(i => i!.Equipo).ThenInclude(e => e!.Marca)
                .Include(m => m.Instalacion).ThenInclude(i => i!.Equipo).ThenInclude(e => e!.Modelo)
                .Include(m => m.Instalacion).ThenInclude(i => i!.Equipo).ThenInclude(e => e!.TipoEquipo)
                .Include(m => m.Instalacion).ThenInclude(i => i!.Area).ThenInclude(a => a!.Empresa)
                .Include(m => m.Instalacion).ThenInclude(i => i!.Area).ThenInclude(a => a!.DireccionEmpresa)
                .Include(m => m.Tecnico)
                .Include(m => m.Encargado)
                .Include(m => m.Supervisor)
                .Include(m => m.TipoMantenimiento)
                .Include(m => m.Estado)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (x == null) return null;

            var equipo = x.Instalacion?.Equipo;
            var area = x.Instalacion?.Area;

            var m = new InformeDatosModel
            {
                Titulo = "INFORME DE MANTENIMIENTO",
                NumeroInforme = Num(x.NumeroInforme, x.Id),
                Estado = x.Estado?.NombreEstado,
                EstadoPie = x.FechaProximo.HasValue ? $"PRÓXIMO MANTENIMIENTO: {x.FechaProximo:yyyy-MM-dd}" : null,
                Observaciones = JoinObs(x.ObservacionInicial, x.ObservacionesFinales)
            };

            m.Secciones.Add(new InformeSeccion { Titulo = "Datos del Servicio" }
                .Add("Empresa", area?.Empresa?.Nombre)
                .Add("Responsable", NombreCompleto(x.Tecnico))
                .Add("Persona encargada", NombreCompleto(x.Encargado))
                .Add("Supervisor", NombreCompleto(x.Supervisor))
                .Add("Tipo de mantenimiento", x.TipoMantenimiento?.NombreTipoMantenimiento)
                .Add("Dirección", area?.DireccionEmpresa?.Direccion)
                .Add("Fecha inicial", x.FechaInicio)
                .Add("Fecha final", x.FechaFin)
                .Add("Requiere repuestos", x.RequiereRepuestos));

            m.Secciones.Add(new InformeSeccion { Titulo = "Datos del Equipo" }
                .Add("Equipo", equipo?.Nombre)
                .Add("Tipo", equipo?.TipoEquipo?.Nombre)
                .Add("No. de serie", equipo?.NumeroSerie)
                .Add("Marca", equipo?.Marca?.Nombre)
                .Add("Modelo", equipo?.Modelo?.Nombre)
                .Add("Área", area?.NombreArea)
                .Add("Capacidad", equipo?.Modelo?.Capacidad)
                .Add("Código", equipo?.Codigo)
                .Add("Potencia", equipo?.Modelo?.Potencia));

            // Trabajos realizados (catálogo trabajo vinculado a este mantenimiento)
            var trabajos = await _context.Trabajos
                .Where(t => t.MantenimientoId == x.Id && t.Activo)
                .OrderBy(t => t.Id).ToListAsync();
            if (trabajos.Count > 0)
            {
                var sec = new InformeSeccion { Titulo = "Trabajos Realizados", AnchoCompleto = true };
                foreach (var t in trabajos)
                    sec.Campos.Add(new InformeCampo
                    {
                        Etiqueta = t.NombreTrabajo,
                        Valor = string.IsNullOrWhiteSpace(t.Descripcion) ? "Realizado" : t.Descripcion!
                    });
                m.Secciones.Add(sec);
            }

            m.Mediciones = await BuildMedicionesAsync(x.Id, equipo?.Id);
            m.Fotos = await FotosMantenimientoAsync(x.Id);
            m.SupervisorNombre = NombreCompleto(x.Supervisor) ?? NombreCompleto(x.Tecnico);
            m.EncargadoNombre = NombreCompleto(x.Encargado);

            return await _render.RenderTemplateAsync("InformeDatos", m);
        }

        public async Task<string?> MantenimientoFotograficoAsync(int id)
        {
            var x = await _context.Mantenimientos
                .Include(m => m.TipoMantenimiento)
                .Include(m => m.Tecnico)
                .Include(m => m.Encargado)
                .Include(m => m.Supervisor)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (x == null) return null;

            var m = new InformeFotograficoModel
            {
                Titulo = "INFORME FOTOGRÁFICO DE MANTENIMIENTO",
                NumeroInforme = Num(x.NumeroInforme, x.Id),
                Descripcion = string.IsNullOrWhiteSpace(x.TipoMantenimiento?.NombreTipoMantenimiento)
                    ? x.ObservacionInicial
                    : $"Mantenimiento {x.TipoMantenimiento!.NombreTipoMantenimiento}. {x.ObservacionInicial}".Trim(),
                Observaciones = x.ObservacionesFinales
            };
            m.Grupos = await FotosMantenimientoAsync(x.Id);

            m.SupervisorNombre = NombreCompleto(x.Supervisor) ?? NombreCompleto(x.Tecnico);
            m.EncargadoNombre = NombreCompleto(x.Encargado);

            return await _render.RenderTemplateAsync("InformeFotografico", m);
        }

        // ════════════════════════════════════════════════════════════════════════
        // VISITA TÉCNICA
        // ════════════════════════════════════════════════════════════════════════
        public async Task<string?> VisitaDatosAsync(int id)
        {
            var v = await _context.VisitasTecnicas
                .Include(x => x.Empresa)
                .Include(x => x.Tecnico)
                .Include(x => x.TipoTrabajo)
                .FirstOrDefaultAsync(x => x.Id == id);
            if (v == null) return null;

            var m = new InformeDatosModel
            {
                Titulo = "INFORME DE VISITA TÉCNICA",
                NumeroInforme = Num(v.NumeroInforme, v.Id),
                Observaciones = v.Observaciones
            };

            m.Secciones.Add(new InformeSeccion { Titulo = "Datos de la Visita", AnchoCompleto = true }
                .Add("Empresa", v.Empresa?.Nombre)
                .Add("Responsable", NombreCompleto(v.Tecnico))
                .Add("Tipo de trabajo", v.TipoTrabajo?.NombreTipoTrabajo)
                .Add("Fecha de visita", v.FechaVisita)
                .Add("Descripción", v.DescripcionVisita));

            m.Fotos = await FotosVisitaAsync(v.Id);
            m.SupervisorNombre = NombreCompleto(v.Tecnico);

            return await _render.RenderTemplateAsync("InformeDatos", m);
        }

        public async Task<string?> VisitaFotograficoAsync(int id)
        {
            var v = await _context.VisitasTecnicas
                .Include(x => x.TipoTrabajo)
                .Include(x => x.Tecnico)
                .FirstOrDefaultAsync(x => x.Id == id);
            if (v == null) return null;

            var m = new InformeFotograficoModel
            {
                Titulo = "INFORME FOTOGRÁFICO DE VISITA TÉCNICA",
                NumeroInforme = Num(v.NumeroInforme, v.Id),
                Descripcion = string.IsNullOrWhiteSpace(v.TipoTrabajo?.NombreTipoTrabajo)
                    ? v.DescripcionVisita
                    : $"{v.TipoTrabajo!.NombreTipoTrabajo}. {v.DescripcionVisita}".Trim(),
                Observaciones = v.Observaciones
            };
            m.Grupos = await FotosVisitaAsync(v.Id);

            m.SupervisorNombre = NombreCompleto(v.Tecnico);

            return await _render.RenderTemplateAsync("InformeFotografico", m);
        }

        private static string? JoinObs(string? inicial, string? final)
        {
            var partes = new List<string>();
            if (!string.IsNullOrWhiteSpace(inicial)) partes.Add($"Inicial: {inicial}");
            if (!string.IsNullOrWhiteSpace(final)) partes.Add($"Final: {final}");
            return partes.Count == 0 ? null : string.Join("\n", partes);
        }
    }
}
