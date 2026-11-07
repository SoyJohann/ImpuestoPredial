using Coactivos_Predial.Models;
using Coactivos_Predial.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace Coactivos_Predial.Shared.AppDbContext;

public class TenantDbContext(DbContextOptions<TenantDbContext> options) : DbContext(options)
{
    public DbSet<Predio> Predio => Set<Predio>();
    public DbSet<Propietarios> Propietarios => Set<Propietarios>();
    public DbSet<TiposIdentificacion> TiposIdentificacion => Set<TiposIdentificacion>();
    public DbSet<Parametros> Parametros => Set<Parametros>();
    public DbSet<Importados> Importados => Set<Importados>();
    public DbSet<Documentos> Documentos => Set<Documentos>();
    public DbSet<Procesos> Procesos => Set<Procesos>();
    public DbSet<Contribuyente> Contribuyentes => Set<Contribuyente>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Predio>()
            .HasOne(predio => predio.Propietario)
            .WithMany(propietario => propietario.Predios)
            .HasForeignKey(predio => predio.PropietarioId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<PrediosProp>()
            .HasOne(relacion => relacion.Predio)
            .WithMany(predio => predio.Copropietarios)
            .HasForeignKey(relacion => relacion.MaestroId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<PrediosProp>()
            .HasOne(relacion => relacion.Propietario)
            .WithMany()
            .HasForeignKey(relacion => relacion.PropietarioId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Predio>()
            .HasIndex(predio => predio.Codigo)
            .IsUnique();

        modelBuilder.Entity<Propietarios>()
            .HasIndex(propietario => new { propietario.TipoId, propietario.Documento })
            .IsUnique();

        modelBuilder.Entity<Propietarios>()
            .HasOne(propietario => propietario.TipoIdentificacion)
            .WithMany(tipo => tipo.Propietarios)
            .HasForeignKey(propietario => propietario.TipoId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Documentos>()
            .HasOne(documento => documento.Predio)
            .WithMany(predio => predio.Documentos)
            .HasForeignKey(documento => documento.PredioId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Documentos>()
            .Property(documento => documento.Modulo)
            .HasConversion<int>();

        modelBuilder.Entity<Documentos>()
            .HasOne(documento => documento.Contribuyente)
            .WithMany(contribuyente => contribuyente.Documentos)
            .HasForeignKey(documento => documento.ContribuyenteId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Contribuyente>()
            .HasOne(c => c.Propietario)
            .WithMany(p => p.Contribuyentes)
            .HasForeignKey(c => c.PropietarioId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Contribuyente>()
            .Property(c => c.Estado)
            .HasConversion<int>();

        modelBuilder.Entity<Documentos>()
            .HasIndex(d => new { d.Modulo, d.IsActive });

        modelBuilder.Entity<Procesos>()
            .Property(proceso => proceso.TipoProceso)
            .HasConversion<int>();

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            foreach (var property in entityType.GetProperties().Where(property =>
                         (property.ClrType == typeof(DateTime) || property.ClrType == typeof(DateTime?))
                         && property.Name != nameof(EntityWithTraceability.CreatedAt)
                         && property.Name != nameof(EntityWithTraceability.UpdatedAt)))
            {
                property.SetColumnType("date");
            }

            if (!typeof(EntityWithTraceability).IsAssignableFrom(entityType.ClrType))
            {
                continue;
            }

            var entity = modelBuilder.Entity(entityType.ClrType);
            entity.Property(nameof(EntityWithTraceability.CreatedAt))
                .HasColumnType("timestamp with time zone")
                .HasDefaultValueSql("now()");
            entity.Property(nameof(EntityWithTraceability.UpdatedAt))
                .HasColumnType("timestamp with time zone");
            entity.Property(nameof(EntityWithTraceability.CreatedBy))
                .HasDefaultValue(1);
            entity.Property(nameof(EntityWithTraceability.UpdatedBy))
                .HasDefaultValue(1);
            entity.Property(nameof(EntityWithTraceability.IsActive))
                .HasDefaultValue(true);
        }
    }
}
