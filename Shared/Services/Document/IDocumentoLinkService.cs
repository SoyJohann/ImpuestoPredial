namespace Coactivos_Predial.Shared.Services.Document;

public interface IDocumentoLinkService
{
    Task<DocumentoLinks?> GetLinksAsync(int documentId);
}