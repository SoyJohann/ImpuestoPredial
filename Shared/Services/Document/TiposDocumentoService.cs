using Coactivos_Predial.Shared.AppDbContext;
using Coactivos_Predial.Shared.Services.Document.Enums;
using Microsoft.EntityFrameworkCore;

namespace Coactivos_Predial.Shared.Services.Document;

public sealed record TipoDocumentoDto(int Id, string Nombre, int Orden);

public interface ITiposDocumentoService
{
    Task<List<TipoDocumentoDto>> GetActivosAsync(Modulo modulo, CancellationToken cancellationToken = default);
    Task<TipoDocumentoDto?> GetAsync(int id, Modulo modulo, CancellationToken cancellationToken = default);
}

/// <summary>Consulta el catálogo configurable de tipos documentales por módulo.</summary>
public sealed class TiposDocumentoService : ITiposDocumentoService
{
    private readonly TenantDbContext _tenantDb;

    public TiposDocumentoService(TenantDbContext tenantDb) => _tenantDb = tenantDb;

    public async Task<List<TipoDocumentoDto>> GetActivosAsync(Modulo modulo, CancellationToken cancellationToken = default) =>
        await _tenantDb.TiposDocumento
            .AsNoTracking()
            .Where(tipo => tipo.Modulo == modulo && tipo.Activo && tipo.IsActive)
            .OrderBy(tipo => tipo.Orden)
            .ThenBy(tipo => tipo.Nombre)
            .Select(tipo => new TipoDocumentoDto(tipo.Id, tipo.Nombre, tipo.Orden))
            .ToListAsync(cancellationToken);

    public async Task<TipoDocumentoDto?> GetAsync(int id, Modulo modulo, CancellationToken cancellationToken = default) =>
        await _tenantDb.TiposDocumento
            .AsNoTracking()
            .Where(tipo => tipo.Id == id && tipo.Modulo == modulo && tipo.Activo && tipo.IsActive)
            .Select(tipo => new TipoDocumentoDto(tipo.Id, tipo.Nombre, tipo.Orden))
            .FirstOrDefaultAsync(cancellationToken);
}
