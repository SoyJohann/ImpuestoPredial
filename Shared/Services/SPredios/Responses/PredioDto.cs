namespace Coactivos_Predial.Shared.Services.SPredios.Responses;

/// <summary>Datos del predio que se muestran en la consulta.</summary>
public sealed record PredioDto
{
    public int Id { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Direccion { get; set; } = string.Empty;
    public string? Matricula { get; set; }
    public int? AreaTerreno { get; set; }
    public int? AreaConstruida { get; set; }
    public bool? EsMunicipio { get; set; }
    public bool? Exento { get; set; }
    public double? Avaluo { get; set; }
    public string? Estrato { get; set; }
    public bool? Desenglobo { get; set; }
    public string? Zona { get; set; }
    public int? Barrio { get; set; }
    public bool? Ley550 { get; set; }
    public string? PropietarioNombre { get; set; }
    public string? PropietarioDocumento { get; set; }
}
