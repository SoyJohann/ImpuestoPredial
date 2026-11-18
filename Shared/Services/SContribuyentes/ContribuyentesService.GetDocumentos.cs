using Coactivos_Predial.Shared.Services.Common;
using Coactivos_Predial.Shared.Services.Document.Enums;
using Coactivos_Predial.Shared.Services.SPDios.Responses;
using Microsoft.EntityFrameworkCore;

namespace Coactivos_Predial.Shared.Services.SContribuyentes;

public partial class ContribuyentesService
{
    public Task<Result<List<DocumentoDto>>> GetDocumentos(int contribuyenteId) =>
        ResultExecution.ExecuteAsync(async () =>
        {
            return await tenantDbContext.Documentos
                .AsNoTracking()
                .Where(documento =>
                    documento.ContribuyenteId == contribuyenteId
                    && documento.Modulo == Modulo.IndustriaComercio
                    && documento.IsActive
                    && documento.EliminadoEn == null)
                .OrderBy(documento => documento.Id)
                .Select(documento => new DocumentoDto(
                    documento.Id,
                    documento.Modulo,
                    documento.TipoDocumentoId,
                    documento.TipoDocumento.Nombre,
                    documento.PredioId,
                    documento.CodigoPredial,
                    documento.Nombre,
                    documento.NombreArchivo,
                    documento.Fecha,
                    documento.Ruta))
                .ToListAsync();
        }, "No fue posible consultar los documentos del contribuyente.");
}
