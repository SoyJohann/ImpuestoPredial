namespace Coactivos_Predial.Shared.Services.SImportados.Responses;

/// <summary>Fila temporal importada desde el archivo catastral.</summary>
public sealed record ImportadoDto(
    int Id,
    string Codigo,
    string? Cedula,
    string Nombre,
    string Direccion,
    string? Matricula,
    int? AreaTerreno,
    int? AreaConstruida,
    int? Tipo,
    bool? EsMunicipio,
    bool? Exento,
    double? Avaluo,
    string? Estrato,
    bool? Desenglobo,
    string? Zona);
