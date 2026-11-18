using System.Security.Claims;
using Coactivos_Predial.Shared.Models;
using Coactivos_Predial.Shared.Components.Ui;
using Coactivos_Predial.Shared.Services.Document;
using Coactivos_Predial.Shared.Services.Document.Enums;
using Coactivos_Predial.Shared.Services.SPredios.Requests;
using Coactivos_Predial.Shared.Services.SPredios.Responses;
using Coactivos_Predial.Shared.Services.SPDios.Responses;
using Coactivos_Predial.Shared.Services.SPropietarios.Responses;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;

namespace Coactivos_Predial.Pages.PConsulta;

public partial class Consulta
{
    // null = todavía no se ha encontrado ningún predio.
    private PredioDto? Predio { get; set; }

    // Lo que escribe el usuario en el buscador (el DTO es de solo lectura).
    private string codigoPredial = string.Empty;

    private List<ProcesoDto> ProcesosDto { get; set; } = [];
    private List<PropietarioDto> PropietarioDto { get; set; } = [];
    private List<DocumentoDto> DocumentosDto { get; set; } = [];
    private List<TipoDocumentoDto> TiposDocumento { get; set; } = [];
    private string filtroDocumentos = string.Empty;
    private int? carpetaSeleccionada;
    private string tipoDocumentoSeleccionado = string.Empty;
    private string archivoSeleccionado = string.Empty;
    private PdfViewerModal? pdfViewerModal;

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

    private int TipoSeleccionadoId =>
        int.TryParse(tipoDocumentoSeleccionado, out var tipoId) ? tipoId : 0;

    protected override async Task OnInitializedAsync()
    {
        await Tenant.InitializeAsync();
        await CargarTiposAsync();
    }

    private async Task CargarTiposAsync()
    {
        TiposDocumento = await tiposDocumentoService.GetActivosAsync(Modulo.Predial);
        if (string.IsNullOrWhiteSpace(tipoDocumentoSeleccionado) && TiposDocumento.Count > 0)
        {
            tipoDocumentoSeleccionado = TiposDocumento[0].Id.ToString();
        }
    }

    private Documentos Documentos { get; set; } = new();
    private readonly List<IBrowserFile> loadedFiles = new();
    private const long MaxFileSize = 4 * 1024 * 1024;
    private readonly List<string> errors = [];

    private bool isLoading;
    private string ptotalconrecFormated = string.Empty;
    private string nentidad = string.Empty;
    private bool busquedaRealizada;
    private string? errorBusqueda;
    private string? aviso;
    private bool subiendo;
    private bool mostrarModalSubir;
    private string nestado_proc = string.Empty;
    private bool mostrarModalBusqueda;

    // Estado de los diálogos de la interfaz.
    private DocumentoDto? docPendienteEliminar;
    private bool mostrarConfirmacionEliminar;

    // para authenticacion
    private const string RequiredDeleteRole = "2";

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

    private string PropietariosResumen => PropietarioDto.Count == 0
        ? "Sin propietarios"
        : $"{PropietarioDto.Count} propietario{(PropietarioDto.Count == 1 ? "" : "s")}";

    // ---------------------------------------------------------------- Búsqueda

    private Task Consultar() => EjecutarConsultaAsync(codigoPredial);

    private async Task SeleccionarDesdeModal(string codigoCapturado)
    {
        mostrarModalBusqueda = false;
        await EjecutarConsultaAsync(codigoCapturado);
    }

    private void ShowBusqueda() => mostrarModalBusqueda = true;

    private void CerrarModalBusqueda() => mostrarModalBusqueda = false;

    private async Task EjecutarConsultaAsync(string? codigoIngresado)
    {
        var codigo = codigoIngresado?.Trim();
        if (string.IsNullOrWhiteSpace(codigo))
        {
            errorBusqueda = "Ingrese o seleccione un código predial para consultar.";
            busquedaRealizada = false;
            return;
        }

        // Limpiar para próximas consultas
        codigoPredial = codigo;
        errorBusqueda = null;
        busquedaRealizada = true;
        Predio = null;
        DocumentosDto.Clear();
        PropietarioDto.Clear();
        ProcesosDto.Clear();
        carpetaSeleccionada = null;
        filtroDocumentos = string.Empty;
        nentidad = string.Empty;
        ptotalconrecFormated = string.Empty;
        nestado_proc = string.Empty;
        isLoading = true;
        StateHasChanged();
        try
        {
            await CargarPredioAsync(codigo);

            if (Predio is not null)
            {
                return;
            }
        }
        finally
        {
            isLoading = false;
        }
    }

    /// <summary>Busca el predio por código (normalmente el código predial).</summary>
    private async Task CargarPredioAsync(string codigoBusqueda)
    {
        var respuesta = await prediosService.GetPrediosByCode(codigoBusqueda);
        if (respuesta.IsFailure)
        {
            errorBusqueda = respuesta.Error ?? "No fue posible consultar el predio.";
            return;
        }

        var predio = respuesta.Value;
        if (predio is null)
        {
            errorBusqueda = "No se encontró ningún predio con ese código.";
            return;
        }

        Predio = predio;
        nentidad = predio.Exento == true ? "Exento" : "Normal";
        ptotalconrecFormated = predio.Avaluo?.ToString("C") ?? string.Empty;

        await CargarDocumentosAsync(predio.Id);
    }

    // ---------------------------------------------------------------- Subida de documentos

    private void LimpiarCarga(int? tipoId = null)
    {
        var tipoInicial = tipoId is int id && TiposDocumento.Any(tipo => tipo.Id == id)
            ? id
            : TiposDocumento.FirstOrDefault()?.Id ?? 0;

        Documentos = new Documentos
        {
            TipoDocumentoId = tipoInicial,
            Nombre = tipoId is null ? string.Empty : NombreCarpeta(tipoInicial)
        };
        tipoDocumentoSeleccionado = tipoInicial.ToString();
        archivoSeleccionado = string.Empty;
        loadedFiles.Clear();
        errors.Clear();
    }

    private void AbrirModalSubir(int? tipoId = null)
    {
        LimpiarCarga(tipoId ?? carpetaSeleccionada);
        mostrarModalSubir = true;
    }

    private void CerrarModalSubir() => mostrarModalSubir = false;

    private async Task ArchivoSeleccionado(InputFileChangeEventArgs e)
    {
        if (subiendo)
        {
            return;
        }

        await LoadFiles(e);
    }

    private async Task SubirDocumento()
    {
        var predio = Predio;

        // Evita que dos clics rápidos creen dos registros durante la primera petición.
        if (predio is null || subiendo || !ValidarDocumentoSeleccionado())
        {
            return;
        }

        Documentos.PredioId = predio.Id;
        var tipoSubido = TipoSeleccionadoId;
        subiendo = true;
        var documentosAntes = DocumentosDto.Count;
        await InvokeAsync(StateHasChanged);
        await Task.Yield();

        try
        {
            await GuardarDocumentoAsync(predio.Id);
        }
        finally
        {
            subiendo = false;
        }

        if (DocumentosDto.Count > documentosAntes)
        {
            carpetaSeleccionada = tipoSubido;
            LimpiarCarga(tipoSubido);
            mostrarModalSubir = false;
            aviso = "Documento cargado correctamente.";
        }
    }

    private async Task GuardarDocumentoAsync(int predioId)
    {
        if (loadedFiles.Count != 1)
        {
            errors.Add("Seleccione un solo archivo con extensión .pdf.");
            return;
        }

        await using var contenido = loadedFiles[0].OpenReadStream(MaxFileSize);
        var request = new AgregarDocumentoRequest
        {
            PredioId = predioId,
            Modulo = Modulo.Predial,
            TipoDocumentoId = TipoSeleccionadoId,
            Nombre = Documentos.Nombre,
            NombreArchivo = loadedFiles[0].Name,
            Contenido = contenido,
            Fecha = Documentos.Fecha ?? DateTime.Today
        };

        var resultado = await prediosService.AgregarDocumento(request);
        if (resultado.IsFailure)
        {
            errorBusqueda = resultado.Error ?? "No fue posible guardar el documento.";
            Console.WriteLine($"Error en agregar el pdf: {resultado.Error}");
            return;
        }

        await CargarDocumentosAsync(predioId);
    }

    private async Task LoadFiles(InputFileChangeEventArgs e)
    {
        try
        {
            errors.Clear();
            loadedFiles.Clear();
            archivoSeleccionado = string.Empty;

            if (e.FileCount != 1)
            {
                errors.Add("Seleccione un solo archivo PDF.");
                return;
            }

            var file = e.File;
            if (!string.Equals(Path.GetExtension(file.Name), ".pdf", StringComparison.OrdinalIgnoreCase))
            {
                errors.Add("El archivo debe tener extensión .pdf.");
                return;
            }

            if (file.Size <= 0 || file.Size > MaxFileSize)
            {
                errors.Add("El PDF debe pesar más de 0 y hasta 4 MB.");
                return;
            }

            archivoSeleccionado = Path.GetFileName(file.Name);
            loadedFiles.Add(file);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Unexpected error in LoadFiles: {ex.Message}");
            throw;
        }
    }

    private bool ValidarDocumentoSeleccionado()
    {
        errors.Clear();

        if (TipoSeleccionadoId <= 0
            || !TiposDocumento.Any(tipo => tipo.Id == TipoSeleccionadoId))
        {
            errors.Add("Seleccione un tipo de documento.");
        }

        if (string.IsNullOrWhiteSpace(Documentos.Nombre))
        {
            errors.Add("Ingrese el detalle del documento.");
        }

        if (loadedFiles.Count != 1 || string.IsNullOrWhiteSpace(archivoSeleccionado))
        {
            errors.Add("Seleccione un solo archivo con extensión .pdf.");
        }
        else if (loadedFiles[0].Size <= 0 || loadedFiles[0].Size > MaxFileSize)
        {
            errors.Add("El PDF debe pesar más de 0 y hasta 4 MB.");
        }

        return errors.Count == 0;
    }

    // ---------------------------------------------------------------- Documentos

    private void CerrarAviso() => aviso = null;

    private void AbrirCarpeta(int tipoId)
    {
        carpetaSeleccionada = tipoId;
        filtroDocumentos = string.Empty;
    }

    private void CerrarCarpeta()
    {
        carpetaSeleccionada = null;
        filtroDocumentos = string.Empty;
    }

    private string NombreCarpeta(int tipoId) =>
        TiposDocumento.FirstOrDefault(tipo => tipo.Id == tipoId)?.Nombre ?? "Sin clasificar";

    private static string FechaTexto(DateTime? fecha) =>
        fecha is null || fecha == default || fecha.Value.Year <= 1
            ? "—"
            : fecha.Value.ToString("dd/MM/yyyy");

    private async Task CargarDocumentosAsync(int predioId)
    {
        var resultado = await prediosService.GetPdfsByCode(predioId);
        if (resultado.IsSuccess)
        {
            DocumentosDto = resultado.Value ?? new List<DocumentoDto>();
        }
        else
        {
            DocumentosDto.Clear();
            errorBusqueda = resultado.Error ?? "No fue posible consultar los documentos del predio.";
        }
    }

    private Task ActualizarDocumentos() =>
        Predio is { } predio ? CargarDocumentosAsync(predio.Id) : Task.CompletedTask;

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

    // ---------------------------------------------------------------- Eliminar

    private async Task PermisoEliminar(DocumentoDto docs)
    {
        var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
        var user = authState.User;

        if (user.Identity?.IsAuthenticated == true)
        {
            var userHasAccess = user.Claims.Any(c => c.Type == ClaimTypes.Role && c.Value == RequiredDeleteRole);

            if (userHasAccess)
            {
                docPendienteEliminar = docs;
                mostrarConfirmacionEliminar = true;
            }
            else
            {
                await JSRuntime.InvokeVoidAsync("alert", "No tienes permisos para eliminar documentos");
            }
        }
    }

    private void CancelarEliminar()
    {
        docPendienteEliminar = null;
        mostrarConfirmacionEliminar = false;
    }

    private async Task ConfirmarEliminar()
    {
        if (docPendienteEliminar is null)
        {
            return;
        }

        var docs = docPendienteEliminar;

        // Eliminar el documento de la base de datos
        var eliminacion = await prediosService.DeleteDocs(docs.Id);
        if (eliminacion.IsFailure)
        {
            errorBusqueda = eliminacion.Error ?? "No fue posible eliminar el documento.";
            docPendienteEliminar = null;
            mostrarConfirmacionEliminar = false;
            return;
        }

        docPendienteEliminar = null;
        mostrarConfirmacionEliminar = false;

        // Actualizar la lista de documentos
        if (Predio is { } predio)
        {
            await CargarDocumentosAsync(predio.Id);
        }
    }

    // ---------------------------------------------------------------- Propietarios y procesos

    private async Task CargarProcesosAsync(string codigo)
    {
        try
        {
            var respuesta = await prediosService.GetCoactivo(codigo);
            ProcesosDto = respuesta.Value ?? new List<ProcesoDto>();
            if (respuesta.IsFailure)
            {
                Console.WriteLine($"Error en getresoluciones: {respuesta.Error}");
                return;
            }

            nestado_proc = "Estado : Sin Proceso";
            foreach (var item in ProcesosDto)
            {
                nestado_proc = (string.IsNullOrWhiteSpace(item.NombreEstado) ? "Sin proceso" : item.NombreEstado.Trim())
                               + " Resol: "
                               + (string.IsNullOrWhiteSpace(item.Resolucion) ? "—" : item.Resolucion.Trim())
                               + " Del : "
                               + FechaTexto(item.FechaMandamiento);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error en getresoluciones: {ex.Message}");
        }
    }

    private async Task CargarPropietariosAsync(string codigo)
    {
        try
        {
            var respuesta = await propietariosService.PropietariosBypredial(codigo);
            PropietarioDto = respuesta.Value ?? new List<PropietarioDto>();
            if (respuesta.IsFailure)
            {
                Console.WriteLine($"Error en getprop: {respuesta.Error}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error en getprop: {ex.Message}");
        }
    }
}