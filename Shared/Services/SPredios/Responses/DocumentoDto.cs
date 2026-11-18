namespace Coactivos_Predial.Shared.Services.SPDios.Responses;

/// <summary>Documento PDF asociado a un predio o contribuyente.</summary>
public sealed record DocumentoDto(
    int Id,
    Document.Enums.Modulo Modulo,
    int TipoDocumentoId,
    string TipoDocumentoNombre,
    int? PredioId,
    string? CodigoPredial,
    string Nombre,
    string? NombreArchivo,
    DateTime? Fecha,
    string? Ruta);
