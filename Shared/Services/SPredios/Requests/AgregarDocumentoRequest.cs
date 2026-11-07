using Coactivos_Predial.Shared.Services.Document.Enums;

namespace Coactivos_Predial.Shared.Services.SPredios.Requests;

/// <summary>Datos para registrar un documento. El tipo documental
/// determina el sector y la carpeta de almacenamiento.</summary>
public sealed class AgregarDocumentoRequest
{
    public int PredioId { get; set; }
    public Modulo  Modulo { get; set; }
    public string Nombre { get; set; } = null!;
    public string NombreArchivo { get; set; } = null!;
    public DateTime? Fecha { get; set; }
}
