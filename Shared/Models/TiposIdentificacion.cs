using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Coactivos_Predial.Shared.Models;

/// <summary>Catálogo de tipos de documento con los que se identifican las personas.</summary>
[Table("TiposIdentificacion")]
public class TiposIdentificacion
{
    [Key]
    public int Id { get; set; }

    public string Nombre { get; set; } = string.Empty;

    /// <summary>Propietarios identificados con este tipo de documento.</summary>
    public ICollection<Propietarios> Propietarios { get; set; } = new List<Propietarios>();
}
