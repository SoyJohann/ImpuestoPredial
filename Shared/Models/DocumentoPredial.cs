namespace Coactivos_Predial.Shared.Models;

/// <summary>
/// Proyección de solo lectura que usan los reportes de documentos prediales consultados con Dapper.
/// </summary>
public class DocumentoPredial
{
    public int Id { get; set; }
    public string CodigoPredial { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public DateTime Fecha { get; set; }
    public string Ruta { get; set; } = string.Empty;
    public int Num { get; set; }
}
