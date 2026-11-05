namespace Coactivos_Predial.Shared.Services.SPredios.Responses;

/// <summary>Proyección de lectura con los datos usados en la búsqueda de predios.</summary>
public sealed record PredioBusquedaDto(
    string CodigoPredial,
    double Avaluo,
    string? Direccion,
    string? Matricula,
    string? Nombre);
