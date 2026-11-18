using System.ComponentModel.DataAnnotations.Schema;
using Coactivos_Predial.Shared.Services.Document.Enums;

namespace Coactivos_Predial.Shared.Models;

/// <summary>
/// Representa un archivo documental clasificado y asociado a un predio cuando se pudo identificarlo.
/// </summary>
public class Documentos : EntityWithTraceability
{
    [Column("Modulo")]
    public Modulo Modulo { get; set; } = Modulo.Predial;

    /// <summary>Tipo documental configurable al que pertenece el documento.</summary>
    [Column("TipoDocumentoId")]
    public int TipoDocumentoId { get; set; }

    [Column("PredioId")]
    public int? PredioId { get; set; }

    /// <summary>Código histórico conservado únicamente para documentos sin predio asociado.</summary>
    [Column("CodigoPredial")]
    public string? CodigoPredial { get; set; }

    /// <summary>Descripción libre del documento (el antiguo "detalle").</summary>
    [Column("Nombre")]
    public string Nombre { get; set; } = string.Empty;

    /// <summary>Nombre original saneado que se usa para mostrar y descargar. Nunca forma la ruta física.</summary>
    [Column("NombreArchivo")]
    public string? NombreArchivo { get; set; }

    /// <summary>Clave física del archivo (GUID + extensión). Null en documentos heredados.</summary>
    [Column("StorageKey")]
    public string? StorageKey { get; set; }

    [Column("MimeType")]
    public string? MimeType { get; set; }

    /// <summary>Tamaño real del archivo en bytes, calculado en el servidor.</summary>
    [Column("Size")]
    public long? Size { get; set; }

    [Column("HashSha256")]
    public string? HashSha256 { get; set; }

    [Column("Fecha", TypeName = "date")]
    public DateTime? Fecha { get; set; }

    /// <summary>Ruta heredada (solo nombre de archivo o ruta absoluta antigua).</summary>
    [Column("Ruta")]
    public string? Ruta { get; set; }

    [Column("ContribuyenteId")]
    public int? ContribuyenteId { get; set; }

    /// <summary>Fecha de eliminación lógica. Solo la modifica el servicio de documentos.</summary>
    [Column("EliminadoEn", TypeName = "timestamp with time zone")]
    public DateTime? EliminadoEn { get; set; }

    /// <summary>Usuario que realizó la eliminación lógica. Solo lo modifica el servicio de documentos.</summary>
    [Column("EliminadoPor")]
    public int? EliminadoPor { get; set; }

    [ForeignKey(nameof(TipoDocumentoId))]
    public TipoDocumento TipoDocumento { get; set; } = null!;

    [ForeignKey(nameof(PredioId))]
    public Predio? Predio { get; set; }

    [ForeignKey(nameof(ContribuyenteId))]
    public Contribuyente? Contribuyente { get; set; }
}
