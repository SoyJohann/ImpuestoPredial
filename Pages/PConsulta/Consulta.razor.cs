using System.Security.Claims;
using Coactivos_Predial.Shared.Models;
using Coactivos_Predial.Shared.Components.Ui;
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
    private string filtroDocumentos = string.Empty;
    private PdfViewerModal? pdfViewerModal;

    private IEnumerable<DocumentoDto> DocumentosFiltrados => string.IsNullOrWhiteSpace(filtroDocumentos)
        ? DocumentosDto
        : DocumentosDto.Where(doc =>
            (doc.Nombre ?? string.Empty).Contains(filtroDocumentos, StringComparison.OrdinalIgnoreCase)
            || (doc.Ruta ?? string.Empty).Contains(filtroDocumentos, StringComparison.OrdinalIgnoreCase));

    private Documentos Documentos { get; set; } = new();
    private readonly List<IBrowserFile> loadedFiles = new();
    private const long MaxFileSize = 4 * 1024 * 1024;
    private readonly List<string> errors = [];
    private string nombreArchivo = string.Empty;

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

    private string DocumentosResumen => DocumentosDto.Count == 0
        ? "Sin documentos"
        : $"{DocumentosDto.Count} documento{(DocumentosDto.Count == 1 ? "" : "s")}";

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

    private void LimpiarCarga()
    {
        Documentos = new Documentos();
        loadedFiles.Clear();
        errors.Clear();
        nombreArchivo = codigoPredial;
    }

    private void AbrirModalSubir()
    {
        LimpiarCarga();
        mostrarModalSubir = true;
    }

    private void CerrarModalSubir() => mostrarModalSubir = false;

    private async Task ArchivoSeleccionado(InputFileChangeEventArgs e)
    {
        if (subiendo)
        {
            return;
        }

        nombreArchivo = codigoPredial;
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
            Documentos = new Documentos();
            loadedFiles.Clear();
            errors.Clear();
            nombreArchivo = codigoPredial;
            mostrarModalSubir = false;
            aviso = "Documento cargado correctamente.";
        }
    }

    private async Task GuardarDocumentoAsync(int predioId)
    {
        var request = new AgregarDocumentoRequest
        {
            PredioId = predioId,
            Modulo = Modulo.Predial,
            Nombre = Documentos.Nombre,
            NombreArchivo = Documentos.Ruta,
            Fecha = Documentos.Fecha ?? DateTime.Today
        };

        var resultado = await prediosService.AgregarDocumento(request);
        if (resultado.IsFailure)
        {
            errorBusqueda = resultado.Error ?? "No fue posible guardar el documento.";
            Console.WriteLine($"Error en agregar el pdf: {resultado.Error}");
            return;
        }

        await GuardarArchivoEnSegundoPlano();
        await CargarDocumentosAsync(predioId);
    }

    private async Task GuardarArchivoEnSegundoPlano()
    {
        try
        {
            var stringStorage = _connectionService.GetStorageStringForTipo(Modulo.Predial).Trim();
            Directory.CreateDirectory(stringStorage);

            var file = loadedFiles[0];
            var fullFilePath = Path.Combine(stringStorage, Documentos.Ruta);
            await using var fileStream = new FileStream(fullFilePath, FileMode.Create);
            await file.OpenReadStream(MaxFileSize).CopyToAsync(fileStream);
        }
        catch (Exception ex)
        {
            await JSRuntime.InvokeVoidAsync("alert", $"Error al guardar el archivo en segundo plano: {ex.Message}");
        }
    }

    private async Task LoadFiles(InputFileChangeEventArgs e)
    {
        try
        {
            errors.Clear();
            loadedFiles.Clear();
            Documentos.Ruta = string.Empty;

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

            var originalFileName = Path.GetFileNameWithoutExtension(file.Name);
            Documentos.Ruta = $"{nombreArchivo}_{originalFileName}.pdf";
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

        if (string.IsNullOrWhiteSpace(Documentos.Nombre))
        {
            errors.Add("Ingrese el detalle del documento.");
        }

        if (loadedFiles.Count != 1
            || string.IsNullOrWhiteSpace(Documentos.Ruta)
            || !string.Equals(Path.GetExtension(Documentos.Ruta), ".pdf", StringComparison.OrdinalIgnoreCase))
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

        // Eliminar el archivo físico
        try
        {
            if (File.Exists(docs.Ruta))
            {
                File.Delete(docs.Ruta);
                Console.WriteLine($"Archivo {docs.Ruta} eliminado correctamente.");
            }
            else
            {
                Console.WriteLine($"El archivo {docs.Ruta} no existe.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al eliminar el archivo {docs.Ruta}: {ex.Message}");
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