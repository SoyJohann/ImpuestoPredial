using Coactivos_Predial.Shared.Services.Document.Enums;

namespace Coactivos_Predial.Shared.Services.SPredios.Requests;

/// <summary>Datos para registrar un documento. El módulo determina el sector,
/// el tipo documental determina la carpeta virtual y el contenido se valida en el servidor.</summary>
public sealed class AgregarDocumentoRequest
{
    public int PredioId { get; set; }
    public int? ContribuyenteId { get; set; }
    public Modulo Modulo { get; set; }
    public int TipoDocumentoId { get; set; }

    /// <summary>Descripción libre (el antiguo "detalle").</summary>
    public string Nombre { get; set; } = null!;

    /// <summary>Nombre original del archivo tal como lo entrega el navegador.</summary>
    public string NombreArchivo { get; set; } = null!;

    /// <summary>Contenido real del PDF. El servidor calcula tamaño y hash.</summary>
    public Stream? Contenido { get; set; }

    public DateTime? Fecha { get; set; }
}
