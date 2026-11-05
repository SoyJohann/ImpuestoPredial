namespace Coactivos_Predial.Shared.Models;

/// <summary>
/// Proyección Dapper de la base municipal anterior; no forma una tabla del contexto code-first.
/// </summary>
public class Persuasivos
{
    public int Id { get; set; }
    public string Comparendo { get; set; } = string.Empty;
    public string Cedula { get; set; } = string.Empty;
    public DateTime Fecha { get; set; }
    public string Nombres { get; set; } = string.Empty;
    public string Infraccion { get; set; } = string.Empty;
    public string Direccion { get; set; } = string.Empty;
    public string Telefono { get; set; } = string.Empty;
    public double Valor { get; set; }
    public string Ciudad { get; set; } = string.Empty;
    public DateTime FechaVencimiento { get; set; }
    public string Departamento { get; set; } = string.Empty;
}
