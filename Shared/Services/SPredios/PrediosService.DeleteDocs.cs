using Coactivos_Predial.Shared.Services.Common;
using Microsoft.EntityFrameworkCore;

namespace Coactivos_Predial.Shared.Services.SPredios;

public partial class PrediosService
{
    public Task<Result> DeleteDocs(int id) =>
        ResultExecution.ExecuteAsync(async () =>
        {
            var documento = await tenantDbContext.Documentos!
                .FirstOrDefaultAsync(documento => documento.Id == id && documento.IsActive);

            if (documento is null)
            {
                throw new KeyNotFoundException("El documento no existe o ya fue eliminado.");
            }

            var userId = await userContext.GetUserIdAsync();
            if (userId <= 0)
            {
                throw new UnauthorizedAccessException("No se pudo identificar al usuario autenticado.");
            }

            documento.IsActive = false;
            documento.UpdatedBy = userId;
            documento.UpdatedAt = DateTime.UtcNow;

            await tenantDbContext.SaveChangesAsync();
        }, "No fue posible eliminar el documento.");
}
