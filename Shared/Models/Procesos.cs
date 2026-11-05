using System.ComponentModel.DataAnnotations;

namespace Coactivos_Predial.Shared.Models;

public class Procesos
{
    [Key] public int id { get; set; }
    public string codigo { get; set; }
    public string cedula { get; set; }
    public DateTime fecha_mdto { get; set; }
    public int estado { get; set; }
    public int desde { get; set; }
    public int hasta { get; set; }
    public string nestado { get; set; }
    public float valor { get; set; }
    public string resolucion { get; set; }

    public string expediente { get; set; }
    public DateTime fecha_proc { get; set; }
}