namespace Coactivos_Predial.Shared.Services.SPredios.Responses;

/// <summary>Datos del predio que se muestran en la consulta.</summary>
public sealed record PredioDto(
    int Id,
    string Codigo,
    string Direccion,
    string? Matricula,
    int? AreaTerreno,
    int? AreaConstruida,
    bool? EsMunicipio,
    bool? Exento,
    double? Avaluo,
    string? Estrato,
    bool? Desenglobo,
    string? Zona,
    int? Barrio,
    bool? Ley550,
    string? PropietarioNombre,
    string? PropietarioDocumento);