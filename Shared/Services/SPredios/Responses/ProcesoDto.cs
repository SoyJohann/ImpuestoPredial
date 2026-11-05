namespace Coactivos_Predial.Shared.Services.SPredios.Responses;

/// <summary>Proceso de cobro coactivo asociado a un predio.</summary>
public sealed record ProcesoDto(
    int Id,
    string? Codigo,
    string? Cedula,
    string? Comparendo,
    DateTime? FechaMandamiento,
    string? Estado,
    DateTime? Desde,
    DateTime? Hasta,
    string? NombreEstado,
    double? Valor,
    string? Resolucion,
    string? Expediente,
    DateTime? FechaProceso);
