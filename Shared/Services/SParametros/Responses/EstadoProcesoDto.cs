namespace Coactivos_Predial.Shared.Services.SParametros.Responses;

/// <summary>Estado del proceso y porcentaje del valor adicional.</summary>
public sealed record EstadoProcesoDto
{
    public int id { get; set; }
    public string nombre { get; set; } = string.Empty;
    public float porc { get; set; }
}
