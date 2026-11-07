using Coactivos_Predial.Shared.Components.Ui;
using Coactivos_Predial.Shared.Services.SDatabase;
using Coactivos_Predial.Shared.Services.SEstadisticas;
using Microsoft.AspNetCore.Components;

namespace Coactivos_Predial.Pages.PInicio;

public partial class Inicio
{
    [Inject] private IEstadisticasService Estadisticas { get; set; } = default!;
    [Inject] private ITenantProvider Tenant { get; set; } = default!;

    private int? documentosPredial;
    private string? error;
    private bool cargando = true;

    protected override async Task OnInitializedAsync()
    {
        await Tenant.InitializeAsync();

        try
        {
            cargando = true;
            var resultado = await Estadisticas.GetResumenDocumentosAsync();

            if (resultado.IsSuccess)
            {
                documentosPredial = resultado.Value?.Predial;
                error = null;
            }
            else
            {
                documentosPredial = null;
                error = resultado.Error ?? "No se pudo obtener el resumen de documentos.";
            }
        }
        finally
        {
            cargando = false;
        }
    }
}
