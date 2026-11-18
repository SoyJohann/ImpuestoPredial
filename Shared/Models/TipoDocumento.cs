using System.ComponentModel.DataAnnotations.Schema;
using Coactivos_Predial.Shared.Services.Document.Enums;

namespace Coactivos_Predial.Shared.Models;

/// <summary>
/// Tipo documental configurable por módulo. Define la carpeta virtual a la que
/// pertenece un documento y el orden en que se presenta al usuario.
/// </summary>
public class TipoDocumento : EntityWithTraceability
{
    [Column("Modulo")]
    public Modulo Modulo { get; set; } = Modulo.Predial;

    [Column("Nombre")]
    public string Nombre { get; set; } = string.Empty;

    [Column("Orden")]
    public int Orden { get; set; }

    /// <summary>Indica si el tipo se ofrece para nuevas cargas. No reemplaza a IsActive.</summary>
    [Column("Activo")]
    public bool Activo { get; set; } = true;

    public ICollection<Documentos> Documentos { get; set; } = new List<Documentos>();
}
