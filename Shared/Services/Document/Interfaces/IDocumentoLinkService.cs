using Coactivos_Predial.Shared.Services.Document.Reponses;

namespace Coactivos_Predial.Shared.Services.Document.Interfaces;

public interface IDocumentoLinkService
{
    Task<DocumentoLinksDto?> GetLinksAsync(int documentId);
}