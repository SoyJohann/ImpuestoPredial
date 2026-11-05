using System.ComponentModel.DataAnnotations.Schema;

namespace Coactivos_Predial.Shared.Models;

/// <summary>
/// Representa un archivo documental clasificado y asociado a un predio cuando se pudo identificarlo.
/// </summary>
[Table("Documentos")]
public class Documentos
{
    [Column("Id")]
    public int Id { get; set; }

    [Column("TipoDocumentoId")]
    public int TipoDocumentoId { get; set; }

    [Column("PredioId")]
    public int? PredioId { get; set; }

    /// <summary>Código histórico conservado únicamente para documentos sin predio asociado.</summary>
    [Column("CodigoPredial")]
    public string? CodigoPredial { get; set; }

    [Column("Nombre")]
    public string Nombre { get; set; } = string.Empty;

    [Column("Fecha", TypeName = "date")]
    public DateTime? Fecha { get; set; }

    [Column("Ruta")]
    public string? Ruta { get; set; }

    [Column("Archivo")]
    public string? Archivo { get; set; }

    [ForeignKey(nameof(TipoDocumentoId))]
    public TipoDocumento TipoDocumento { get; set; } = null!;

    [ForeignKey(nameof(PredioId))]
    public Maestro? Predio { get; set; }
}
