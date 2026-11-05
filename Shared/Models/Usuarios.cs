using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Coactivos_Predial.Shared.Models;

/// <summary>
/// Representa una cuenta de acceso y su configuración de municipio y permisos.
/// </summary>
[Table("usuarios")]
public class Usuarios
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("login")]
    public string Login { get; set; } = string.Empty;
    [Column("nombre")]
    public string Nombre { get; set; } = string.Empty;
    [Column("habilitado")]
    public bool Habilitado { get; set; }
    [Column("clave")]
    public string Clave { get; set; } = string.Empty;
    [Column("nivel")]
    public short Nivel { get; set; }
    [Column("dir")]
    public string Direccion { get; set; } = string.Empty;
    [Column("tel")]
    public string Telefono { get; set; } = string.Empty;
    [Column("correo")]
    public string? Correo { get; set; }
    [Column("usuario_creo")]
    public int? UsuarioCreoId { get; set; }
    [Column("municipio")]
    public int Municipio { get; set; }
    [Column("mun")]
    public string MunicipioNombre { get; set; } = string.Empty;
}
