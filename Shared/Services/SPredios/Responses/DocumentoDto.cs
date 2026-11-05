namespace Coactivos_Predial.Shared.Services.SPredios.Responses;

/// <summary>Documento PDF asociado a un predio.</summary>
public sealed record DocumentoDto(
    int Id,
    int TipoDocumentoId,
    int? PredioId,
    string? CodigoPredial,
    string Nombre,
    DateTime? Fecha,
    string? Ruta,
    string? Archivo);
