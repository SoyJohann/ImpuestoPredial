namespace Coactivos_Predial.Shared.Services.SPredios.Responses;

/// <summary>Fila del reporte diario de documentos cargados.</summary>
public sealed record DocumentoDiarioDto(string CodigoPredial, DateTime Fecha, int Num);
