using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Coactivos_Predial.Shared.Models;

public class DocumentoTipoHistorial
{
    [Key]
    public int Id { get; set; }

    public int DocumentoId { get; set; }

    public int TipoAnteriorId { get; set; }

    public int TipoNuevoId { get; set; }

    public int UsuarioId { get; set; }

    public DateTime Fecha { get; set; }
}
