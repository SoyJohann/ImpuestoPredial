namespace Coactivos_Predial.Shared.Services.SPDios.Responses;

/// <summary>Documento PDF asociado a un predio.</summary>
public sealed record DocumentoDto(
    int Id,
    Document.Enums.Modulo Modulo,
    int? PredioId,
    string? CodigoPredial,
    string Nombre,
    DateTime? Fecha,
    string? Ruta,
    string? Archivo);
