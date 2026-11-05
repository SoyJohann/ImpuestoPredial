using Coactivos_Predial.Models;
using Coactivos_Predial.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace Coactivos_Predial.Shared.AppDbContext;

/// <summary>
/// Define el esquema de datos de un municipio: catastro, propietarios, procesos,
/// parámetros y documentos. La base se selecciona por el proveedor del municipio.
/// </summary>
public class TenantDbContext(DbContextOptions<TenantDbContext> options) : DbContext(options)
{
    public DbSet<Maestro> Maestro => Set<Maestro>();
    public DbSet<Propietarios> Propietarios => Set<Propietarios>();
    public DbSet<TiposIdentificacion> TiposIdentificacion => Set<TiposIdentificacion>();
    public DbSet<PrediosProp> PrediosProp => Set<PrediosProp>();
    public DbSet<Parametros> Parametros => Set<Parametros>();
    public DbSet<Importados> Importados => Set<Importados>();
    public DbSet<Documentos> Documentos => Set<Documentos>();
    public DbSet<Sectores> Sectores => Set<Sectores>();
    public DbSet<TipoDocumento> TiposDocumento => Set<TipoDocumento>();
    public DbSet<Procesos> Procesos => Set<Procesos>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Maestro>()
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

        modelBuilder.Entity<Maestro>()
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

        modelBuilder.Entity<Sectores>()
            .HasIndex(sector => sector.Codigo)
            .IsUnique();

        modelBuilder.Entity<TipoDocumento>()
            .HasIndex(tipo => new { tipo.SectorId, tipo.Codigo })
            .IsUnique();

        modelBuilder.Entity<Documentos>()
            .HasOne(documento => documento.Predio)
            .WithMany(predio => predio.Documentos)
            .HasForeignKey(documento => documento.PredioId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Documentos>()
            .HasOne(documento => documento.TipoDocumento)
            .WithMany(tipo => tipo.Documentos)
            .HasForeignKey(documento => documento.TipoDocumentoId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Procesos>()
            .Property(proceso => proceso.TipoProceso)
            .HasConversion<int>();

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            foreach (var property in entityType.GetProperties().Where(property =>
                         property.ClrType == typeof(DateTime) || property.ClrType == typeof(DateTime?)))
            {
                property.SetColumnType("date");
            }
        }
    }
}
