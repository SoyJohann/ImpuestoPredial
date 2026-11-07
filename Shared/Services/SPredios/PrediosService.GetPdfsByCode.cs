using Coactivos_Predial.Shared.Services.Common;
using Coactivos_Predial.Shared.Services.Document.Enums;
using Coactivos_Predial.Shared.Services.SPDios.Responses;
using Microsoft.EntityFrameworkCore;

namespace Coactivos_Predial.Shared.Services.SPredios;

public partial class PrediosService
{
    public Task<Result<List<DocumentoDto>>> GetPdfsByCode(int predioId) =>
        ResultExecution.ExecuteAsync(async () =>
        {
            return await tenantDbContext.Documentos!
                .AsNoTracking()
                .Where(s => s.PredioId == predioId && s.Modulo == Modulo.Predial)
                .OrderBy(documento => documento.Id)
                .Select(documento => new DocumentoDto(
                    documento.Id,
                    documento.Modulo,
                    documento.PredioId,
                    documento.CodigoPredial,
                    documento.Nombre,
                    documento.Fecha,
                    documento.Ruta,
                    documento.Ruta))
                .ToListAsync();
        }, "No fue posible consultar los documentos del predio.");
}
