using Coactivos_Predial.Shared.Services.Common;
using Coactivos_Predial.Shared.Services.SPredios.Responses;
using Microsoft.EntityFrameworkCore;

namespace Coactivos_Predial.Shared.Services.SPredios;

public partial class PrediosService
{
    public Task<Result<List<DocumentoDto>>> GetPdfsByCode(int predioId) =>
        ResultExecution.ExecuteAsync(async () =>
        {
            return await tenantDbContext.Documentos!
                .AsNoTracking()
                .Where(s=>s.PredioId == predioId && s.TipoDocumentoId == 1) //Arreglar a un enum porque 1 queda ilegible.
                .OrderBy(documento => documento.Id)
                .Select(documento => new DocumentoDto(
                    documento.Id,
                    documento.TipoDocumentoId,
                    documento.PredioId,
                    documento.CodigoPredial,
                    documento.Nombre,
                    documento.Fecha,
                    documento.Ruta,
                    documento.Archivo))
                .ToListAsync();
        }, "No fue posible consultar los documentos del predio.");
}
