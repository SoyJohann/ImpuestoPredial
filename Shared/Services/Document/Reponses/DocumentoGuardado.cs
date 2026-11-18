namespace Coactivos_Predial.Shared.Services.Document.Reponses;

public sealed record DocumentoGuardado(
    string StorageKey,
    string RutaCompleta,
    long Size,
    string HashSha256,
    string MimeType);