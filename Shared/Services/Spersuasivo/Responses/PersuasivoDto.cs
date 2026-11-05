namespace Coactivos_Predial.Shared.Services.Spersuasivo.Responses;

/// <summary>Proyección de un proceso persuasivo para listados.</summary>
public sealed record PersuasivoDto(
    int Id,
    string Comparendo,
    string Cedula,
    DateTime Fecha,
    string Nombres,
    string Infraccion,
    string Direccion,
    string Telefono,
    double Valor,
    string Ciudad,
    DateTime FechaVencimiento,
    string Departamento);
