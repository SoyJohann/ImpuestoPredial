namespace Coactivos_Predial.Shared.Services.SUser.Responses;

/// <summary>Datos de una cuenta de acceso para listados y consultas.</summary>
public sealed record UsuarioDto
{
    public int Id { get; set; }
    public string Login { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public bool Habilitado { get; set; }
    public string Clave { get; set; } = string.Empty;
    public int Nivel { get; set; }
    public string Direccion { get; set; } = string.Empty;
    public string Telefono { get; set; } = string.Empty;
    public string? Correo { get; set; }
    public int? UsuarioCreoId { get; set; }
    public int Municipio { get; set; }
    public string MunicipioNombre { get; set; } = string.Empty;
}
