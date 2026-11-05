using System.ComponentModel.DataAnnotations.Schema;

namespace Coactivos_Predial.Shared.Models;

/// <summary>
/// Agrupa los módulos documentales del sistema, como Predial e Industria y Comercio.
/// </summary>
[Table("Sectores")]
public class Sectores
{
    [Column("Id")]
    public int Id { get; set; }

    [Column("Codigo")]
    public string Codigo { get; set; } = string.Empty;

    [Column("Nombre")]
    public string Nombre { get; set; } = string.Empty;

    [Column("Activo")]
    public bool Activo { get; set; } = true;

    public ICollection<TipoDocumento> TiposDocumento { get; set; } = new List<TipoDocumento>();
}
