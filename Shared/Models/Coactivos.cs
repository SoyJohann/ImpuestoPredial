using System.ComponentModel.DataAnnotations;

namespace Coactivos_Predial.Shared.Models;

/// <summary>
/// Proyección Dapper de la base municipal anterior; no forma una tabla del contexto code-first.
/// </summary>
public class Coactivos
{
    [Key]
    public int Id { get; set; }

    public string? Codigo { get; set; }
    public string? Cedula { get; set; }
    public string? Resolucion { get; set; }
    public string? PrimerosNombres { get; set; }
    public string? Direccion { get; set; }
    public DateTime Desde { get; set; }
    public DateTime Hasta { get; set; }
    public int SemestreUno { get; set; }
    public int SemestreDos { get; set; }
    public DateTime Fecha { get; set; }
    public DateTime FechaSancion { get; set; }
    public DateTime FechaMandamiento { get; set; }
    public DateTime FechaProceso { get; set; }
    public double Valor { get; set; }
}
