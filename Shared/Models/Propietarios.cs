using System.ComponentModel.DataAnnotations;

namespace Coactivos_Predial.Shared.Models;

public class Propietarios
{
    [Key]
    public int Id { get; set; }

    [Required, StringLength(20)]
    public string Documento { get; set; } = string.Empty;

    public int TipoId { get; set; }

    public TiposIdentificacion TipoIdentificacion { get; set; } = null!;

    [StringLength(120)]
    public string Nombre { get; set; } = string.Empty;

    public string? EstadoCivil { get; set; }
    public string? Telefono { get; set; }
    public string? Correo { get; set; }
    public bool? Fallecido { get; set; }

    public ICollection<Predio> Predios { get; set; } = new List<Predio>();

    public ICollection<Contribuyente> Contribuyentes { get; set; } = new List<Contribuyente>();
}