using Coactivos_Predial.Shared.AppDbContext;
using Coactivos_Predial.Shared.Services.Common;
using Coactivos_Predial.Shared.Models;
using Coactivos_Predial.Shared.Services.UserAccesor;
using Microsoft.EntityFrameworkCore;
namespace Coactivos_Predial.Shared.Services.SDocumentos;

/// <summary>
/// Servicio de operaciones sobre el modelo Documentos.
/// Incluye cambio de tipo (un lote y individual) con validación de módulos,
/// activación y historial básico.
/// </summary>
public sealed class SDocumentosService(TenantDbContext tenantDb, IUserContextService userContext)
{
    /// <summary>
    /// Cambia el tipo documental de un documento único.
    /// </summary>
    public async Task<Result> CambiarTipoAsync(int documentoId, int nuevoTipoId)
    {
        var userId = await userContext.GetUserIdAsync();
        if (userId <= 0)
            return Result.Fail("No se pudo identificar al usuario.", ResultErrorType.Unauthorized);

        // Validar tipo nuevo: activo y con módulo
        var nuevoTipo = await tenantDb.TiposDocumento!
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == nuevoTipoId && t.Activo && t.IsActive);

        if (nuevoTipo is null)
            return Result.Fail("Tipo documental no válido o inactivo.", ResultErrorType.Validation);

        // Buscar documento: activo y no eliminado
        var documento = await tenantDb.Documentos!
            .FirstOrDefaultAsync(d => d.Id == documentoId && d.IsActive && d.EliminadoEn == null);

        if (documento is null)
            return Result.Fail("Documento no existe o ya está eliminado.", ResultErrorType.NotFound);

        // Validar mismo módulo
        if (nuevoTipo.Modulo != documento.Modulo)
            return Result.Fail("El tipo no pertenece al módulo del documento.", ResultErrorType.Validation);

        // Registrar en historial (tabla DocumentoTipoHistorial)
        await tenantDb.DocumentoTipoHistorial!.AddAsync(new DocumentoTipoHistorial
        {
            DocumentoId = documento.Id,
            TipoAnteriorId = documento.TipoDocumentoId,
            TipoNuevoId = nuevoTipoId,
            UsuarioId = userId,
            Fecha = DateTime.UtcNow
        });

        // Cambiar tipo
        documento.TipoDocumentoId = nuevoTipoId;
        documento.UpdatedBy = userId;
        documento.UpdatedAt = DateTime.UtcNow;

        await tenantDb.SaveChangesAsync();
        return Result.Ok();
    }

    /// <summary>
    /// Cambia el tipo documental de múltiples documentos en una sola operación.
    /// Valida: mismo módulo en todos los documentos, tipo activo, documentos activos/no eliminados.
    /// Rechaza si algún documento tiene módulo distinto al nuevo tipo.
    /// Usa ExecuteUpdateAsync para actualizar en lote y luego inserta historial.
    /// </summary>
    public async Task<Result> CambiarTipoMasivoAsync(IReadOnlyCollection<int> documentoIds, int nuevoTipoId)
    {
        if (documentoIds is null || documentoIds.Count == 0)
            return Result.Fail("No se seleccionaron documentos.", ResultErrorType.Validation);

        var userId = await userContext.GetUserIdAsync();
        if (userId <= 0)
            return Result.Fail("No se pudo identificar al usuario.", ResultErrorType.Unauthorized);

        // Validar tipo nuevo
        var nuevoTipo = await tenantDb.TiposDocumento!
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == nuevoTipoId && t.Activo && t.IsActive);

        if (nuevoTipo is null)
            return Result.Fail("Tipo documental no válido o inactivo.", ResultErrorType.Validation);

        // Traer documentos válidos (activos, no eliminados) y verificar counts
        var documentos = await tenantDb.Documentos!
            .Where(d => documentoIds.Contains(d.Id) && d.IsActive && d.EliminadoEn == null)
            .ToListAsync();

        if (documentos.Count != documentoIds.Count)
            return Result.Fail("Algunos documentos no están en condiciones de ser reclasificados.", ResultErrorType.Validation);

        // Verificar que todos tengan el mismo módulo
        var moduloDocs = documentos.Select(d => d.Modulo).Distinct().ToList();
        if (moduloDocs.Count > 1)
            return Result.Fail("No se pueden reclasificar documentos de módulos diferentes en un lote.", ResultErrorType.Validation);

        // Verificar que el nuevo tipo pertenece al mismo módulo
        if (nuevoTipo.Modulo != moduloDocs[0])
            return Result.Fail("El tipo no pertenece al módulo de los documentos seleccionados.", ResultErrorType.Validation);

        // Actualizar en lote usando ExecuteUpdateAsync
        await tenantDb.Documentos!
            .Where(d => documentoIds.Contains(d.Id))
            .ExecuteUpdateAsync(set => set
                .SetProperty(d => d.TipoDocumentoId, nuevoTipoId)
                .SetProperty(d => d.UpdatedBy, userId)
                .SetProperty(d => d.UpdatedAt, DateTime.UtcNow));

        // Insertar historial por cada documento
        var historiales = documentos
            .Select(d => new DocumentoTipoHistorial
            {
                DocumentoId = d.Id,
                TipoAnteriorId = d.TipoDocumentoId, // valor capturado antes de la actualización en memoria
                TipoNuevoId = nuevoTipoId,
                UsuarioId = userId,
                Fecha = DateTime.UtcNow
            }).ToList();

        await tenantDb.DocumentoTipoHistorial!.AddRangeAsync(historiales);
        await tenantDb.SaveChangesAsync();

        return Result.Ok();
    }
}
