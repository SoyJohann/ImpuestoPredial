using Coactivos_Predial.Shared.Components.Ui;
using Coactivos_Predial.Shared.Models;
using Coactivos_Predial.Shared.Services.Document;
using Coactivos_Predial.Shared.Services.SContribuyentes;
using Coactivos_Predial.Shared.Services.SPDios.Responses;
using Microsoft.JSInterop;

namespace Coactivos_Predial.Pages.PConsulta;

public partial class Consultacontribuyente
{
    private string nit = string.Empty;
    private List<ContribuyenteDto> Contribuyentes { get; set; } = [];
    private ContribuyenteDto? Seleccionado { get; set; }
    private List<DocumentoDto> DocumentosDto { get; set; } = [];
    private string filtroDocumentos = string.Empty;
    private PdfViewerModal? pdfViewerModal;

    private bool isLoading;
    private bool busquedaRealizada;
    private bool mostrarModalBusqueda;
    private string? errorBusqueda;

    private IEnumerable<DocumentoDto> DocumentosFiltrados => string.IsNullOrWhiteSpace(filtroDocumentos)
        ? DocumentosDto
        : DocumentosDto.Where(doc =>
            (doc.Nombre ?? string.Empty).Contains(filtroDocumentos, StringComparison.OrdinalIgnoreCase)
            || (doc.Ruta ?? string.Empty).Contains(filtroDocumentos, StringComparison.OrdinalIgnoreCase));

    private string NegociosResumen => Contribuyentes.Count == 0
        ? "Sin negocios"
        : $"{Contribuyentes.Count} negocio{(Contribuyentes.Count == 1 ? "" : "s")}";

    private string DocumentosResumen => DocumentosDto.Count == 0
        ? "Sin documentos"
        : $"{DocumentosDto.Count} documento{(DocumentosDto.Count == 1 ? "" : "s")}";

    // ---------------------------------------------------------------- Búsqueda

    private Task Consultar() => EjecutarConsultaAsync(nit);

    private async Task SeleccionarContribuyenteDesdeModal(ContribuyenteDto contribuyente)
    {
        mostrarModalBusqueda = false;
        await EjecutarConsultaAsync(contribuyente.PropietarioDocumento, contribuyente.Id);
    }

    private void ShowBusqueda() => mostrarModalBusqueda = true;

    private void CerrarModalBusqueda() => mostrarModalBusqueda = false;

    private async Task EjecutarConsultaAsync(string? nitIngresado, int? contribuyenteId = null)
    {
        var termino = nitIngresado?.Trim();
        if (string.IsNullOrWhiteSpace(termino))
        {
            errorBusqueda = "Ingrese o seleccione un NIT para consultar.";
            busquedaRealizada = false;
            return;
        }

        nit = termino;
        isLoading = true;
        errorBusqueda = null;
        Contribuyentes = [];
        Seleccionado = null;
        DocumentosDto = [];

        try
        {
            var resultado = await contribuyentesService.GetByNit(termino);

            if (resultado.IsFailure)
            {
                errorBusqueda = resultado.Error ?? "No fue posible consultar los contribuyentes.";
            }
            else
            {
                Contribuyentes = resultado.Value ?? [];
                if (Contribuyentes.Count > 0)
                {
                    var elegido = contribuyenteId is int id
                        ? Contribuyentes.FirstOrDefault(contribuyente => contribuyente.Id == id) ?? Contribuyentes[0]
                        : Contribuyentes[0];

                    await Seleccionar(elegido);
                }
            }
        }
        finally
        {
            busquedaRealizada = true;
            isLoading = false;
        }
    }

    private async Task Seleccionar(ContribuyenteDto contribuyente)
    {
        Seleccionado = contribuyente;
        filtroDocumentos = string.Empty;
        errorBusqueda = null;
        await CargarDocumentosAsync(contribuyente.Id);
    }

    private async Task CargarDocumentosAsync(int contribuyenteId)
    {
        var resultado = await contribuyentesService.GetDocumentos(contribuyenteId);

        if (resultado.IsSuccess)
        {
            DocumentosDto = resultado.Value ?? [];
        }
        else
        {
            DocumentosDto = [];
            errorBusqueda = resultado.Error ?? "No fue posible consultar los documentos del contribuyente.";
        }
    }

    private Task ActualizarDocumentos() =>
        Seleccionado is { } contribuyente
            ? CargarDocumentosAsync(contribuyente.Id)
            : Task.CompletedTask;

    // ---------------------------------------------------------------- Visor PDF

    private async Task AbrirPdf(int documentId)
    {
        var links = await DocumentoLinkService.GetLinksAsync(documentId);
        if (links is null)
        {
            await JSRuntime.InvokeVoidAsync("alert", "No se pudo validar el acceso al documento.");
            return;
        }

        if (pdfViewerModal is not null) await pdfViewerModal.OpenAsync(links.ViewUrl, links.DownloadUrl);
    }

    // ---------------------------------------------------------------- Formato

    private static string FechaTexto(DateTime? fecha) =>
        fecha is null || fecha == default || fecha.Value.Year <= 1
            ? "—"
            : fecha.Value.ToString("dd/MM/yyyy");

    private static string EstadoTexto(EstadoContribuyente estado) => estado switch
    {
        EstadoContribuyente.Activo => "Activo",
        EstadoContribuyente.Cerrado => "Cerrado",
        _ => "—"
    };

    private static string EstadoVariant(EstadoContribuyente estado) => estado switch
    {
        EstadoContribuyente.Activo => "ok",
        EstadoContribuyente.Cerrado => "neutral",
        _ => "neutral"
    };
}
