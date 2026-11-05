using System.ComponentModel.DataAnnotations;

namespace Coactivos_Predial.Models;

/// <summary>
/// Contiene los parámetros institucionales y de liquidación usados por el municipio.
/// </summary>
public class Parametros
{
    [Key]
    public int Id { get; set; }

    public string Nit { get; set; } = string.Empty;
    public string Direccion { get; set; } = string.Empty;
    public string? Telefono { get; set; }
    public string? Divipo { get; set; }
    public string Ciudad { get; set; } = string.Empty;

    [Required]
    public string Nombre { get; set; } = string.Empty;

    public DateTime FechaInicioInteres { get; set; }
    public double? ValorRecibo { get; set; }
    public string? Correo { get; set; }
    public DateTime? FechaNuevaLey { get; set; }
    public DateTime? FinNuevaLey { get; set; }
    public bool? CobraAdicional { get; set; }
    public short? CoactivoManual { get; set; }
    public int? DiasPersuasivo { get; set; }
    public int? DiasCoactivo { get; set; }
    public short? DiasMandamiento { get; set; }
    public short? Costas { get; set; }
    public string? CuentaRecaudo { get; set; }
}
