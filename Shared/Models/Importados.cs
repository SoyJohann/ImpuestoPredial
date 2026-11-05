using System.ComponentModel.DataAnnotations;

namespace Coactivos_Predial.Shared.Models;

/// <summary>
/// Representa una fila temporal importada desde el archivo catastral. Conserva
/// los valores originales mientras se valida y procesa la carga.
/// </summary>
public class Importados
{
    [Key]
    public int Id { get; set; }

    public string Codigo { get; set; } = string.Empty;
    public string? Cedula { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Direccion { get; set; } = string.Empty;
    public string? Matricula { get; set; }
    public int? AreaTerreno { get; set; }
    public int? AreaConstruida { get; set; }
    public int? Tipo { get; set; }
    public bool? EsMunicipio { get; set; }
    public bool? Exento { get; set; }
    public double? Avaluo { get; set; }
    public string? Estrato { get; set; }
    public bool? Desenglobo { get; set; }
    public string? Zona { get; set; }
}
