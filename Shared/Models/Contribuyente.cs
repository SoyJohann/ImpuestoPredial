using System.ComponentModel.DataAnnotations;
using Coactivos_Predial.Shared.Models;

namespace Coactivos_Predial.Shared.Models;

/// <summary>
/// Modelo provisional para la demostración de industria y comercio.
/// Los datos de la persona o empresa (documento/NIT, tipo de identificación,
/// nombre, teléfono, correo) viven en la tabla Propietarios.
/// Un propietario puede tener varios negocios (Contribuyentes).
/// </summary>
public class Contribuyente : EntityWithTraceability
{
    public int PropietarioId { get; set; }

    public Propietarios Propietario { get; set; } = null!;

    [StringLength(150)]
    public string? NombreComercial { get; set; }

    [StringLength(30)]
    public string? MatriculaMercantil { get; set; }

    [StringLength(10)]
    public string? ActividadEconomica { get; set; }   // código CIIU

    [StringLength(200)]
    public string? Direccion { get; set; }            // del establecimiento

    public DateTime? FechaInicioActividad { get; set; }

    public EstadoContribuyente Estado { get; set; } = EstadoContribuyente.Activo;

    public ICollection<Documentos> Documentos { get; set; } = new List<Documentos>();
}

/// <summary>
/// Estados posibles de un contribuyente.
/// </summary>
public enum EstadoContribuyente
{
    /// <summary>Contribuyente activo y operando.</summary>
    Activo = 1,

    /// <summary>Contribuyente dado de bajo o cerrado.</summary>
    Cerrado = 2
}

/// <summary>
/// Modelo de transferencia para contribuyentes (solo datos de la vista).
/// </summary>
public sealed record ContribuyenteDto(
    int Id,
    string? NombreComercial,
    string? MatriculaMercantil,
    string? ActividadEconomica,
    string? Direccion,
    DateTime? FechaInicioActividad,
    EstadoContribuyente Estado,
    string PropietarioNombre,
    string PropietarioDocumento
);