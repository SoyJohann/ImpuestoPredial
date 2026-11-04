using System.ComponentModel.DataAnnotations;

namespace Coactivos_Predial.Shared.Models;
public class Coactivos
{
    [Required]
    [Key]
    public int id { get; set; }
    public string? codigo { get; set; }
    public string? cedula { get; set; }
    public string? resolucion { get; set; }
    public string? pnombres { get; set; }
    public string? dir { get; set; }
    public int desde { get; set; }
    public int hasta { get; set; }
    public int sem1 { get; set; }
    public int sem2 { get; set; }
    public DateOnly fecha { get; set; }
    public DateOnly fecha_sancion { get; set; }
    public DateOnly fecha_mdto { get; set; }
    public DateOnly fecha_proceso { get; set; }
    public double valor { get; set; }

}

