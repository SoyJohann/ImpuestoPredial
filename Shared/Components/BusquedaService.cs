namespace Coactivos_Predial.Shared.Components;

public class BusquedaService
{
    private string? ultimoCodigoSeleccionado;

    public Task GuardarCodigoSeleccionado(string codigoPredial)
    {
        ultimoCodigoSeleccionado = codigoPredial;
        return Task.CompletedTask;
    }

    public string ObtenerUltimoCodigoSeleccionado()
    {
        return ultimoCodigoSeleccionado ?? string.Empty;
    }
}
