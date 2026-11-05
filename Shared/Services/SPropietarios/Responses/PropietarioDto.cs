namespace Coactivos_Predial.Shared.Services.SPropietarios.Responses;

/// <summary>Datos de un propietario para listados y consultas.</summary>
public sealed record PropietarioDto
{
    public int Id { get; set; }
    public string Documento { get; set; } = string.Empty;
    public int TipoId { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Telefono { get; set; }
    public string? Correo { get; set; }
    public string? TipoNombre { get; set; }
}
