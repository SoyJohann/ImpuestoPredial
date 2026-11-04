namespace Coactivos_Predial.Shared.Components;

public class BusquedaService
{
    private string? ultimoComparendoCaptured;

    public async Task<string> ComparendoCapturado(string comparendo)
    {
        ultimoComparendoCaptured = comparendo;
        Console.WriteLine($"comparendo desde el servicio: {ultimoComparendoCaptured}");
        return ultimoComparendoCaptured;
    }

    public string ObtenerUltimoComparendoCapturado()
    {
        return ultimoComparendoCaptured;
    }
}