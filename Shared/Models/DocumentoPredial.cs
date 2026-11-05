namespace Coactivos_Predial.Shared.Models;

// DTO de transición para las consultas Dapper que aún usan codigo_predial y archivo.
public class DocumentoPredial
{
    public int Id { get; set; }
    public string CodigoPredial { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public DateTime Fecha { get; set; }
    public string Ruta { get; set; } = string.Empty;
    public int Num { get; set; }
}
