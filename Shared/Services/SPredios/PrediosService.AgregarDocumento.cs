using Coactivos_Predial.Shared.Models;
using Coactivos_Predial.Shared.Services.Common;
using Microsoft.EntityFrameworkCore;

namespace Coactivos_Predial.Shared.Services.SPredios;

public partial class PrediosService
{
    public Task<Result> AgregarDocumento(Documentos documento) =>
        ResultExecution.ExecuteAsync(async () =>
        {
            ArgumentNullException.ThrowIfNull(documento);
            ArgumentException.ThrowIfNullOrWhiteSpace(documento.Nombre);
            ArgumentException.ThrowIfNullOrWhiteSpace(documento.Ruta);

            if (documento.PredioId <= 0)
            {
                throw new InvalidOperationException("Consulte un predio antes de agregarle un documento.");
            }

            if (!string.Equals(Path.GetExtension(documento.Ruta), ".pdf", StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException("El documento debe ser un archivo PDF.", nameof(documento));
            }

            var tipoDocumentoId = await tenantDbContext.TiposDocumento!
                .AsNoTracking()
                .Where(tipo => tipo.Codigo == "POR_CLASIFICAR" && tipo.Sector.Codigo == "PREDIAL")
                .Select(tipo => tipo.Id)
                .SingleOrDefaultAsync();

            if (tipoDocumentoId <= 0)
            {
                throw new InvalidOperationException("No está configurado el tipo documental Predial por clasificar.");
            }

            documento.TipoDocumentoId = tipoDocumentoId;
            documento.Fecha = documento.Fecha == default ? DateTime.Today : documento.Fecha;

            await tenantDbContext.Documentos!.AddAsync(documento);
            await tenantDbContext.SaveChangesAsync();
        }, "No fue posible guardar el documento.");
}
