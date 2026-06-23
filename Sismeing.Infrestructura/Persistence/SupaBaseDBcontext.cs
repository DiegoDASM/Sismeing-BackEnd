using Microsoft.EntityFrameworkCore;
using Sismeing.Domain.Entities.Catalogo;
using Sismeing.Domain.Entities.Operaciones;

namespace Sismeing.Infrestructura.Persistence
{
    public class SupaBaseDBcontext : DbContext
    {
        public SupaBaseDBcontext(DbContextOptions<SupaBaseDBcontext> options) : base(options)
        {
        }

        // ── Catálogos ────────────────────────────────────────────────────────────
        public DbSet<Rol> Roles { get; set; }
        public DbSet<Estado> Estados { get; set; }
        public DbSet<Marca> Marcas { get; set; }
        public DbSet<Modelo> Modelos { get; set; }
        public DbSet<Tipo_Equipo> TiposEquipo { get; set; }
        public DbSet<Tipo_Mantenimiento> TiposMantenimiento { get; set; }
        public DbSet<Tipo_Trabajo> TiposTrabajo { get; set; }
        public DbSet<Tipo_Informe> TiposInforme { get; set; }
        public DbSet<Trabajo> Trabajos { get; set; }

        // ── Operaciones ──────────────────────────────────────────────────────────
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Empresa> Empresas { get; set; }
        public DbSet<Direccion_Empresa> DireccionesEmpresa { get; set; }
        public DbSet<Area_Empresa> AreasEmpresa { get; set; }
        public DbSet<Contrato> Contratos { get; set; }
        public DbSet<Equipo> Equipos { get; set; }
        public DbSet<Instalacion> Instalaciones { get; set; }
        public DbSet<Foto_Instalacion> FotosInstalacion { get; set; }
        public DbSet<Mantenimiento> Mantenimientos { get; set; }
        public DbSet<Foto_Mantenimiento> FotosMantenimiento { get; set; }
        public DbSet<Visita_Tecnica> VisitasTecnicas { get; set; }
        public DbSet<Foto_VisitaTecnica> FotosVisitaTecnica { get; set; }
        public DbSet<Medicion> Mediciones { get; set; }
        public DbSet<Notificacion> Notificaciones { get; set; }
    }
}
