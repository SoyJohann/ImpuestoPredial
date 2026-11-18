using Coactivos_Predial.Shared.Models;
using Coactivos_Predial.Shared.Services.Document.Enums;
using Coactivos_Predial.Shared.Services.Document.Reponses;

namespace Coactivos_Predial.Shared.Services.Document.Interfaces;

public interface IDocumentoStorageService
{
    string CrearStorageKey();
    string ObtenerCarpetaBase(Modulo modulo);
    string ResolverRuta(Modulo modulo, string storageKey);
    bool TryResolver(Documentos documento, out string rutaCompleta);
    Task<DocumentoGuardado> GuardarAsync(Modulo modulo, Stream contenido, CancellationToken cancellationToken);
    void EliminarArchivo(string rutaCompleta);
}