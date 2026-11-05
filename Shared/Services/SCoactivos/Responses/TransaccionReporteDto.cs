namespace Coactivos_Predial.Shared.Services.SCoactivos.Responses;

/// <summary>Transacción generada que se elige en los informes de coactivo.</summary>
public sealed record TransaccionReporteDto(int Transaccion, DateOnly Fecha, int No);
