using Coactivos_Predial.Shared.Services.Document.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Coactivos_Predial.Shared.Models;

/// <summary>
/// Representa un archivo documental clasificado y asociado a un predio cuando se pudo identificarlo.
/// </summary>
public class Documentos : EntityWithTraceability
{
    [Column("Modulo")]
    public Modulo Modulo { get; set; } = Modulo.Predial;

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

    [ForeignKey(nameof(PredioId))]
    public Predio? Predio { get; set; }
}