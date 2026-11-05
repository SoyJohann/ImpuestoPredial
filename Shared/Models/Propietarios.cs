using System.ComponentModel.DataAnnotations;

namespace Coactivos_Predial.Shared.Models;

/// <summary>
/// Guarda una sola vez la identificación y los datos de contacto de una persona
/// propietaria. La relación con los predios se define desde <see cref="Maestro"/>.
/// </summary>
public class Propietarios
{
    [Key]
    public int Id { get; set; }

    [Required, StringLength(20)]
    public string Documento { get; set; } = string.Empty;

    public int TipoId { get; set; }

    /// <summary>Tipo de documento usado para identificar al propietario.</summary>
    public TiposIdentificacion TipoIdentificacion { get; set; } = null!;

    [StringLength(120)]
    public string Nombre { get; set; } = string.Empty;

    public string? EstadoCivil { get; set; }
    public string? Telefono { get; set; }
    public string? Correo { get; set; }
    public bool? Fallecido { get; set; }

    /// <summary>Predios cuyo titular es esta persona.</summary>
    public ICollection<Maestro> Predios { get; set; } = new List<Maestro>();
}
