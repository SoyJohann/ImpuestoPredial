using Coactivos_Predial.Shared.Components.Ui;
using Coactivos_Predial.Shared.Models;
using Coactivos_Predial.Shared.Services.Document;
using Coactivos_Predial.Shared.Services.Document.Enums;
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
    private List<TipoDocumentoDto> TiposDocumento { get; set; } = [];
    private string filtroDocumentos = string.Empty;
    private int? carpetaSeleccionada;
    private PdfViewerModal? pdfViewerModal;

    private bool isLoading;
    private bool busquedaRealizada;
    private bool mostrarModalBusqueda;
    private string? errorBusqueda;

    private IEnumerable<DocumentoDto> DocumentosFiltrados
    {
        get
        {
            var documentos = DocumentosDto.AsEnumerable();
            if (carpetaSeleccionada is { } tipoId)
            {
                documentos = documentos.Where(doc => doc.TipoDocumentoId == tipoId);
            }

            if (!string.IsNullOrWhiteSpace(filtroDocumentos))
            {
                documentos = documentos.Where(doc =>
                    (doc.Nombre ?? string.Empty).Contains(filtroDocumentos, StringComparison.OrdinalIgnoreCase)
                    || (doc.NombreArchivo ?? string.Empty).Contains(filtroDocumentos, StringComparison.OrdinalIgnoreCase)
                    || (doc.Ruta ?? string.Empty).Contains(filtroDocumentos, StringComparison.OrdinalIgnoreCase));
            }

            return documentos;
        }
    }

    private string NegociosResumen => Contribuyentes.Count == 0
        ? "Sin negocios"
        : $"{Contribuyentes.Count} negocio{(Contribuyentes.Count == 1 ? "" : "s")}";

    private string DocumentosResumen
    {
        get
        {
            if (carpetaSeleccionada is { } tipoId)
            {
                var cantidad = DocumentosDto.Count(doc => doc.TipoDocumentoId == tipoId);
                return $"{cantidad} documento{(cantidad == 1 ? "" : "s")} en {NombreCarpeta(tipoId)}";
            }

            return DocumentosDto.Count == 0
                ? "Sin documentos"
                : $"{DocumentosDto.Count} documento{(DocumentosDto.Count == 1 ? "" : "s")}";
        }
    }

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
        carpetaSeleccionada = null;
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

    private async Task DescargarCarpeta(int carpetaId)
    {
        await JSRuntime.InvokeVoidAsync("alert", "Descarga de carpeta no implementada aún.");
    }

    private void AbrirCarpeta(int tipoId)
    {
        carpetaSeleccionada = tipoId;
        filtroDocumentos = string.Empty;
        StateHasChanged();
    }

    private void CerrarCarpeta()
    {
        carpetaSeleccionada = null;
        filtroDocumentos = string.Empty;
        StateHasChanged();
    }

    private string NombreCarpeta(int tipoId) =>
        TiposDocumento.FirstOrDefault(tipo => tipo.Id == tipoId)?.Nombre ?? "Sin clasificar";

    protected override async Task OnInitializedAsync()
    {
        await Tenant.InitializeAsync();
        TiposDocumento = await tiposDocumentoService.GetActivosAsync(Modulo.IndustriaComercio);
    }

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

    private async Task DescargarPdf(int documentId)
    {
        var url = $"/api/documentos/{documentId}/descargar";
        await JSRuntime.InvokeVoidAsync("open", url, "_blank");
    }

    private bool mostrarModalCambiarTipo;
    private DocumentoDto? documentoParaCambiar;
    private string tipoSeleccionadoParaCambiar = string.Empty;

    private void AbrirCambiarTipo(DocumentoDto doc)
    {
        documentoParaCambiar = doc;
        tipoSeleccionadoParaCambiar = doc.TipoDocumentoId.ToString();
        mostrarModalCambiarTipo = true;
    }

    private async Task ConfirmarCambiarTipo()
    {
        if (documentoParaCambiar is null) return;
        if (!int.TryParse(tipoSeleccionadoParaCambiar, out var tipoId) || tipoId <= 0)
        {
            await JSRuntime.InvokeVoidAsync("alert", "Seleccione un tipo válido.");
            return;
        }

        var resultado = await sDocumentosService.CambiarTipoAsync(documentoParaCambiar.Id, tipoId);
        if (resultado.IsSuccess)
        {
            await JSRuntime.InvokeVoidAsync("alert", "Tipo actualizado correctamente");
            if (Seleccionado is { } contribuyente)
            {
                var docs = await contribuyentesService.GetDocumentos(contribuyente.Id);
                DocumentosDto = docs.Value ?? new List<DocumentoDto>();
            }
        }
        else
        {
            await JSRuntime.InvokeVoidAsync("alert", resultado.Error ?? "Error al cambiar tipo");
        }

        mostrarModalCambiarTipo = false;
        documentoParaCambiar = null;
    }

    private void CancelarCambiarTipo()
    {
        mostrarModalCambiarTipo = false;
        documentoParaCambiar = null;
    }

    private async Task CambiarTipoIndividual(DocumentoDto doc)
    {
        AbrirCambiarTipo(doc);
    }

    private async Task CambiarTipoLote()
    {
        var ids = new List<int>();
        // TODO: Implementar selección múltiple con checkboxes
        if (ids.Count == 0)
        {
            await JSRuntime.InvokeVoidAsync("alert", "Seleccione al menos un documento.");
            return;
        }

        var tipoId = await JSRuntime.InvokeAsync<int>("prompt", "Ingrese el ID del nuevo tipo para el lote:");
        if (tipoId > 0)
        {
            var resultado = await sDocumentosService.CambiarTipoMasivoAsync(ids, tipoId);
            if (resultado.IsSuccess)
            {
                await JSRuntime.InvokeVoidAsync("alert", "Tipos actualizados correctamente");
                if (Seleccionado is { } contribuyente)
                {
                    var docs = await contribuyentesService.GetDocumentos(contribuyente.Id);
DocumentosDto = docs.Value ?? new List<DocumentoDto>();
                }
            }
            else
            {
                await JSRuntime.InvokeVoidAsync("alert", resultado.Error ?? "Error al cambiar tipos");
            }
        }
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
