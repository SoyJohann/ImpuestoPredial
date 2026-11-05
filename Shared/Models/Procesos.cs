using System.ComponentModel.DataAnnotations;

namespace Coactivos_Predial.Shared.Models;

/// <summary>
/// Representa un proceso administrativo de cobro. TipoProceso identifica si el proceso
/// es persuasivo o coactivo; los campos exclusivos de cada etapa son opcionales.
/// </summary>
public class Procesos
{
    [Key]
    public int Id { get; set; }

    public TipoProceso TipoProceso { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string? Comparendo { get; set; }
    public string? Cedula { get; set; }
    public string? Nombres { get; set; }
    public string? Infraccion { get; set; }
    public string? Direccion { get; set; }
    public string? Telefono { get; set; }
    public string? Ciudad { get; set; }
    public string? Departamento { get; set; }
    public DateTime? Fecha { get; set; }
    public DateTime? FechaVencimiento { get; set; }
    public DateTime? FechaSancion { get; set; }
    public DateTime? FechaMandamiento { get; set; }
    public DateTime? FechaProceso { get; set; }
    public string? Estado { get; set; }
    public string? NombreEstado { get; set; }
    public DateTime? Desde { get; set; }
    public DateTime? Hasta { get; set; }
    public int? SemestreUno { get; set; }
    public int? SemestreDos { get; set; }
    public double? Valor { get; set; }
    public string? Resolucion { get; set; }
    public string? Expediente { get; set; }
    public bool? Pago { get; set; }
    public double? ResolucionEmbargo { get; set; }
    public DateTime? FechaEmbargo { get; set; }
    public int? Vigencia { get; set; }
    public string? Matricula { get; set; }
    public string? CodigoRap { get; set; }
}
