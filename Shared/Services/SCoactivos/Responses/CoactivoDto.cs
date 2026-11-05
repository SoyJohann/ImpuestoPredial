namespace Coactivos_Predial.Shared.Services.SCoactivos.Responses;

/// <summary>Proyección de un proceso coactivo para listados e informes.</summary>
public sealed record CoactivoDto(
    int Id,
    string? Codigo,
    string? Cedula,
    string? Resolucion,
    string? PrimerosNombres,
    string? Direccion,
    DateTime Desde,
    DateTime Hasta,
    int SemestreUno,
    int SemestreDos,
    DateTime Fecha,
    DateTime FechaSancion,
    DateTime FechaMandamiento,
    DateTime FechaProceso,
    double Valor);
