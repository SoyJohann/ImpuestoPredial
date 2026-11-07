using System.ComponentModel.DataAnnotations;

namespace Coactivos_Predial.Shared.Models;

/// <summary>Asocia propietarios adicionales con un predio para conservar la copropiedad histórica.</summary>
public class PrediosProp
{
    [Key]
    public int Id { get; set; }

    public int MaestroId { get; set; }
    public Predio Predio { get; set; } = null!;

    public int PropietarioId { get; set; }
    public Propietarios Propietario { get; set; } = null!;
}
