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
        public DbSet<TipoEquipo> TiposEquipo { get; set; }
        public DbSet<TipoMantenimiento> TiposMantenimiento { get; set; }
        public DbSet<TipoTrabajo> TiposTrabajo { get; set; }
        public DbSet<TipoInforme> TiposInforme { get; set; }
        public DbSet<Trabajo> Trabajos { get; set; }

        // ── Operaciones ──────────────────────────────────────────────────────────
        public DbSet<Empresa> Empresas { get; set; }
        public DbSet<AreaEmpresa> AreasEmpresa { get; set; }
        public DbSet<DireccionEmpresa> DireccionesEmpresa { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Contrato> Contratos { get; set; }
        public DbSet<Equipo> Equipos { get; set; }
        public DbSet<Instalacion> Instalaciones { get; set; }
        public DbSet<Mantenimiento> Mantenimientos { get; set; }
        public DbSet<Medicion> Mediciones { get; set; }
        public DbSet<FotoInstalacion> FotosInstalacion { get; set; }
        public DbSet<FotoMantenimiento> FotosMantenimiento { get; set; }
        public DbSet<VisitaTecnica> VisitasTecnicas { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ── Mapeo de columnas de auditoría (PascalCase en DB) ────────────────
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                modelBuilder.Entity(entityType.ClrType, b =>
                {
                    b.Property("FechaRegistro").HasColumnName("FechaRegistro");
                    b.Property("UsuarioRegistro").HasColumnName("UsuarioRegistro");
                    b.Property("IpRegistro").HasColumnName("IpRegistro");
                    b.Property("FechaModificacion").HasColumnName("FechaModificacion");
                    b.Property("UsuarioModificacion").HasColumnName("UsuarioModificacion");
                    b.Property("IpModificacion").HasColumnName("IpModificacion");
                    b.Property("FechaEliminacion").HasColumnName("FechaEliminacion");
                    b.Property("UsuarioEliminacion").HasColumnName("UsuarioEliminacion");
                    b.Property("IpEliminacion").HasColumnName("IpEliminacion");
                    b.Property("Activo").HasColumnName("activo");
                });
            }

            // ── Mantenimiento: múltiples FK a Usuario ────────────────────────────
            modelBuilder.Entity<Mantenimiento>(entity =>
            {
                entity.HasOne(m => m.Tecnico)
                      .WithMany(u => u.MantenimientosTecnico)
                      .HasForeignKey(m => m.TecnicoId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(m => m.Supervisor)
                      .WithMany(u => u.MantenimientosSupervisor)
                      .HasForeignKey(m => m.SupervisorId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(m => m.Encargado)
                      .WithMany(u => u.MantenimientosEncargado)
                      .HasForeignKey(m => m.EncargadoId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // ── Contrato: FK a Usuario (encargado) ──────────────────────────────
            modelBuilder.Entity<Contrato>(entity =>
            {
                entity.HasOne(c => c.Encargado)
                      .WithMany(u => u.ContratosEncargado)
                      .HasForeignKey(c => c.EncargadoId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // ── Modelo: validación año de fabricación ────────────────────────────
            modelBuilder.Entity<Modelo>(entity =>
            {
                entity.ToTable(t => t.HasCheckConstraint(
                    "anio_fabricacion_check",
                    "anio_fabricacion >= 1900 AND anio_fabricacion <= 2100"));
            });

            // ── Índices únicos ───────────────────────────────────────────────────
            modelBuilder.Entity<Rol>().HasIndex(r => r.NombreRol).IsUnique();
            modelBuilder.Entity<Estado>().HasIndex(e => e.NombreEstado).IsUnique();
            modelBuilder.Entity<Marca>().HasIndex(m => m.NombreMarca).IsUnique();
            modelBuilder.Entity<TipoEquipo>().HasIndex(t => t.NombreTipo).IsUnique();
            modelBuilder.Entity<TipoMantenimiento>().HasIndex(t => t.NombreTipo).IsUnique();
            modelBuilder.Entity<TipoTrabajo>().HasIndex(t => t.NombreTipo).IsUnique();
            modelBuilder.Entity<TipoInforme>().HasIndex(t => t.NombreTipo).IsUnique();
            modelBuilder.Entity<Usuario>().HasIndex(u => u.Cedula).IsUnique();
            modelBuilder.Entity<Usuario>().HasIndex(u => u.CorreoElectronico).IsUnique();
        }
    }
}
