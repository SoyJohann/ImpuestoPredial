using System.ComponentModel.DataAnnotations;

namespace Coactivos_Predial.Shared.Models;

/// <summary>
/// Representa un predio del maestro catastral. Los datos personales del titular
/// se almacenan en <see cref="Propietarios"/> y se enlazan por <see cref="PropietarioId"/>.
/// </summary>
public class Predio
{
    [Key]
    public int Id { get; set; }

    [Required, StringLength(20)]
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

    /// <summary>Identificador del propietario titular del predio.</summary>
    public int PropietarioId { get; set; }

    /// <summary>Propietario titular; un propietario puede tener varios predios.</summary>
    public Propietarios Propietario { get; set; } = null!;

    /// <summary>Documentos asociados a este predio.</summary>
    public ICollection<Documentos> Documentos { get; set; } = new List<Documentos>();

    /// <summary>Relaciones adicionales de copropiedad que no son el titular principal.</summary>
    public ICollection<PrediosProp> Copropietarios { get; set; } = new List<PrediosProp>();
}
