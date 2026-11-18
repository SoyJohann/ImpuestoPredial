using Coactivos_Predial.Shared.Services.Document.Reponses;

namespace Coactivos_Predial.Shared.Services.Document.Interfaces;

public interface IDocumentoFileService
{
    Task<DocumentoFile?> GetByIdAsync(int id, CancellationToken cancellationToken);
}