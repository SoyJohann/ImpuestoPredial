using System.ComponentModel.DataAnnotations;

namespace Coactivos_Predial.Shared.Services.Document.Enums;

/// <summary>
/// Modulos del sistema documental. Define el módulo (Predial o Industria y Comercio)
/// al que pertenece un documento y de él se deriva la carpeta de almacenamiento.
/// Valores: 10 = Predial, 20 = Industria y Comercio.
/// Agregar un nuevo módulo consiste en declarar un miembro nuevo dentro del rango
/// correspondiente y actualizar la resolución de carpetas en ConnectionService.
/// </summary>
public enum Modulo
{
    [Display(Name = "Predial")] Predial = 10,

    [Display(Name = "Industria y Comercio")] IndustriaComercio = 20,
}