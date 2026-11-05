using System.ComponentModel.DataAnnotations.Schema;

namespace Coactivos_Predial.Shared.Models;

/// <summary>
/// Define una categoría de documento perteneciente a un sector y usada para clasificar archivos.
/// </summary>
[Table("TiposDocumento")]
public class TipoDocumento
{
    [Column("Id")]
    public int Id { get; set; }

    [Column("SectorId")]
    public int SectorId { get; set; }

    [Column("Codigo")]
    public string Codigo { get; set; } = string.Empty;

    [Column("Nombre")]
    public string Nombre { get; set; } = string.Empty;

    [Column("Activo")]
    public bool Activo { get; set; } = true;

    [ForeignKey(nameof(SectorId))]
    public Sectores Sector { get; set; } = null!;

    public ICollection<Documentos> Documentos { get; set; } = new List<Documentos>();
}
