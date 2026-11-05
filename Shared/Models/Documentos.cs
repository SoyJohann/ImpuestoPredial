using System.ComponentModel.DataAnnotations.Schema;

namespace Coactivos_Predial.Shared.Models;

[Table("Documentos")]
public class Documentos
{
    [Column("Id")]
    public int Id { get; set; }

    [Column("TipoDocumentoId")]
    public int TipoDocumentoId { get; set; }

    [Column("PredioId")]
    public int? PredioId { get; set; }

    [Column("CodigoPredial")]
    public string CodigoPredial { get; set; } = string.Empty;

    [Column("Nombre")]
    public string Nombre { get; set; } = string.Empty;

    [Column("Fecha", TypeName = "date")]
    public DateTime Fecha { get; set; }

    [Column("Ruta")]
    public string Ruta { get; set; } = string.Empty;

    [Column("Archivo")]
    public string Archivo { get; set; } = string.Empty;

    [ForeignKey(nameof(TipoDocumentoId))]
    public TipoDocumento TipoDocumento { get; set; } = null!;

}
