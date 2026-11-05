using System.Security.Claims;
using Coactivos_Predial.Models;
using Coactivos_Predial.Shared.Models;
using Coactivos_Predial.Shared.Components.Ui;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;

namespace Coactivos_Predial.Pages.PConsulta;

public partial class Consulta
{
    public Predios comp_obj { get; set; } = new Predios { };
    public List<Procesos> procesoslist { get; set; } = new List<Procesos>();
    public List<Propietarios> propietlist { get; set; } = new List<Propietarios>();
    private List<Documentos> listDocs = new List<Documentos>();
    private string filtroDocumentos = string.Empty;
    private PdfViewerModal? pdfViewerModal;

    private IEnumerable<Documentos> DocumentosFiltrados => string.IsNullOrWhiteSpace(filtroDocumentos)
        ? listDocs
        : listDocs.Where(doc =>
            (doc.Nombre ?? string.Empty).Contains(filtroDocumentos, StringComparison.OrdinalIgnoreCase)
            || (doc.Ruta ?? string.Empty).Contains(filtroDocumentos, StringComparison.OrdinalIgnoreCase));

    private Documentos Docsobj = new Documentos();
    public List<Archivo> archivo = new();
    private List<IBrowserFile> loadedFiles = new();

    private long maxFileSize = 1024 * 1024 * 4;
    private List<string> errors = new();
    public string nombreArchivo = "NumeroComparendo";

    IQueryable<Procesos> datap;
    IQueryable<Documentos> datacon;
    IQueryable<Propietarios> dataprop;
    bool isLoading = false;
    bool existe = true;
    public string ptotalFormated;
    public string mres;
    public string ptotalconrecFormated;
    public string pfechaFormated;
    private string pcomp;
    public string nentidad;
    public string nestado;
    private string error;
    private bool busquedaRealizada;
    private string? errorBusqueda;
    private string? aviso;
    private bool subiendo;
    private bool mostrarModalSubir;
    public string nestado_proc = "";
    private bool mostrarModal;
    private bool tab1 = true;
    private bool tab2 = false;
    private bool tab3 = false;
    public string xfile;

    // Estado de los diálogos de la interfaz.
    private Documentos? docPendienteEliminar;
    private bool mostrarConfirmacionEliminar;

    // para authenticacion
    private bool userHasAccess = false;
    private string requiredRole = "2";

    private string DocumentosResumen => listDocs.Count == 0
        ? "Sin documentos"
        : $"{listDocs.Count} documento{(listDocs.Count == 1 ? "" : "s")}";

    private string PropietariosResumen => propietlist.Count == 0
        ? "Sin propietarios"
        : $"{propietlist.Count} propietario{(propietlist.Count == 1 ? "" : "s")}";

    private string CondicionVariant => comp_obj.exento ? "warn" : "ok";

    private async Task Consultar()
    {
        var codigoBuscado = comp_obj.codigo?.Trim();
        if (string.IsNullOrWhiteSpace(codigoBuscado))
        {
            errorBusqueda = "Ingrese un código predial para consultar.";
            busquedaRealizada = false;
            return;
        }

        comp_obj.codigo = codigoBuscado;
        errorBusqueda = null;
        busquedaRealizada = true;
        await obtenerLiquidacion();

        if (!existe)
        {
            LimpiarCarga();
            await getprop();
            await getprocesos();
        }
    }

    private async Task SeleccionarDesdeModal(string codigoCapturado)
    {
        errorBusqueda = null;
        busquedaRealizada = true;
        comp_obj.codigo = codigoCapturado;
        mostrarModalBusqueda = false;
        await obtenerLiquidacion();

        if (!existe)
        {
            LimpiarCarga();
            await getprop();
            await getprocesos();
        }
    }

    private void LimpiarCarga()
    {
        Docsobj = new Documentos { CodigoPredial = comp_obj.codigo };
        loadedFiles.Clear();
        errors.Clear();
        nombreArchivo = comp_obj.codigo;
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

        Docsobj.CodigoPredial = comp_obj.codigo;
        nombreArchivo = comp_obj.codigo;
        await LoadFiles(e);
    }

    private async Task SubirDocumento()
    {
        // Evita que dos clics rápidos creen dos registros durante la primera petición.
        if (subiendo || !ValidarDocumentoSeleccionado())
        {
            return;
        }

        Docsobj.CodigoPredial = comp_obj.codigo;
        Docsobj.PredioId = comp_obj.id;
        subiendo = true;
        var documentosAntes = listDocs.Count;
        await InvokeAsync(StateHasChanged);
        await Task.Yield();

        try
        {
            await Agregar_Pdf();
        }
        finally
        {
            subiendo = false;
        }

        if (listDocs.Count > documentosAntes)
        {
            Docsobj = new Documentos { CodigoPredial = comp_obj.codigo };
            loadedFiles.Clear();
            errors.Clear();
            nombreArchivo = comp_obj.codigo;
            mostrarModalSubir = false;
            aviso = "Documento cargado correctamente.";
        }

        StateHasChanged();
    }

    private void CerrarAviso() => aviso = null;

    private static string FechaTexto(DateOnly fecha) =>
        fecha == default || fecha.Year <= 1 ? "—" : fecha.ToString("dd/MM/yyyy");

    private static string FechaTexto(DateTime fecha) =>
        fecha == default || fecha.Year <= 1 ? "—" : fecha.ToString("dd/MM/yyyy");


    private async Task GetPdfs()
    {
        if (string.IsNullOrWhiteSpace(comp_obj.codigo))
        {
            listDocs.Clear();
            return;
        }

        try
        {
            listDocs = await comparendoService.GetPdfsByComp(comp_obj.codigo.Trim());
            Console.WriteLine($"Consulta de documentos para {comp_obj.codigo}: {listDocs.Count} resultado(s).");
        }
        catch (Exception ex)
        {
            listDocs.Clear();
            errorBusqueda = "No fue posible consultar los documentos del predio.";
            Console.WriteLine($"Error al consultar documentos del predio {comp_obj.codigo}: {ex}");
        }
    }

    private async Task PermisoEliminar(Documentos docs)
    {
        var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
        var user = authState.User;

        if (user.Identity?.IsAuthenticated == true)
        {
            userHasAccess = user.Claims.Any(c => c.Type == ClaimTypes.Role && c.Value == requiredRole);

            if (userHasAccess)
            {
                docPendienteEliminar = docs;
                mostrarConfirmacionEliminar = true;
                StateHasChanged();
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
        await comparendoService.DeleteDocs(docs.Id);

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
        await GetPdfs();
        StateHasChanged();
    }


    private async Task Agregar_Pdf()
    {
        if (string.IsNullOrEmpty(Docsobj.Nombre))
        {
            await JSRuntime.InvokeVoidAsync("alert", "Por favor, ingresa el detalle del documento.");
            return;
        }

        if (string.IsNullOrEmpty(Docsobj.Ruta))
        {
            await JSRuntime.InvokeVoidAsync("alert", "Por favor, escoja un documento a cargar.");
            return;
        }

        try
        {
            var count = await comparendoService.AgregarDocumento(Docsobj);

            // Cerrar el modal
            mostrarModal = false;
            await GetPdfs();
            StateHasChanged();

            // Ejecutar el resto del código en segundo plano
            await GuardarArchivoEnSegundoPlano();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error en agregar el pdf:{ex}");
        }
    }

    private async Task GuardarArchivoEnSegundoPlano()
    {
        try
        {
            if (loadedFiles == null || !loadedFiles.Any())
            {
                await JSRuntime.InvokeVoidAsync("alert", "No hay archivos cargados.");
                return;
            }

            var stringStorage = _connectionService.GetStorageString("FileStorageTemplate").Trim();
            Console.WriteLine($"Ruta para guardar desde la consulta: {stringStorage}");

            foreach (var file in loadedFiles)
            {
                string directoryPath = stringStorage;
                string fullFilePath = Path.Combine(directoryPath, Docsobj.Ruta);

                Console.WriteLine($"directoryPath en guardar en segundo plano: {directoryPath}");

                // Asegurarse de que el directorio existe
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                    Console.WriteLine($"Directorio creado: {directoryPath}");
                }

                // Guardar el archivo en el sistema de archivos
                await using FileStream fs = new FileStream(fullFilePath, FileMode.Create);
                await file.OpenReadStream(maxFileSize).CopyToAsync(fs);
                Console.WriteLine($"Archivo guardado: {fullFilePath}");
            }
        }
        catch (Exception ex)
        {
            await JSRuntime.InvokeVoidAsync("alert", $"Error al guardar el archivo en segundo plano: {ex.Message}");
        }
    }

    private bool mostrarModalBusqueda = false;

    // 1. Mostrar el Modal
    private void ShowBusqueda()
    {
        mostrarModalBusqueda = true;
    }

    // 2. Cerrar el Modal
    private void CerrarModalBusqueda()
    {
        mostrarModalBusqueda = false;
    }

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
    }

    public void displayTabs(int TabNumber)
    {
        switch (TabNumber)
        {
            case 1:
                this.tab1 = true;
                this.tab2 = false;
                this.tab3 = false;
                break;
            case 2:
                this.tab1 = false;
                this.tab2 = true;
                this.tab3 = false;
                break;
            case 3:
                this.tab1 = false;
                this.tab3 = true;
                this.tab2 = false;
                break;
        }
    }

    private async Task obtenerLiquidacion()
    {
        try
        {
            var codigoBuscado = comp_obj.codigo?.Trim();
            if (string.IsNullOrEmpty(codigoBuscado))
            {
                return;
            }

            var result = await comparendoService.GetPrediosByCode(codigoBuscado);
            if (result is not null && !string.IsNullOrWhiteSpace(result.codigo))
            {
                comp_obj = result;
                if (comp_obj.exento)
                {
                    nentidad = "Exento";
                }
                else
                {
                    nentidad = "Normal";
                }
                existe = false;
                await GetPdfs();
                ptotalconrecFormated = (comp_obj.avaluo).ToString("C");
                displayTabs(1);
            }
            else
            {
                // GetMultasByComp históricamente devuelve un objeto vacío cuando no encuentra
                // coincidencia o falla. Conservamos el código consultado para buscar sus PDFs.
                comp_obj.codigo = codigoBuscado;
                await GetPdfs();
                // Los documentos pueden existir aunque la consulta complementaria de maestro
                // no haya devuelto los datos del predio; en ese caso igual mostramos la lista.
                existe = listDocs.Count == 0;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in obtenerLiquidacion: {ex.Message}");
            errorBusqueda = "Ocurrió un error al consultar el predio y sus documentos.";
            existe = true;
        }
    }

    private async Task getprocesos()
    {
        try
        {
            await ShowLoading();
            procesoslist = await comparendoService.GetCoactivo(comp_obj.codigo);
            datap = procesoslist.AsQueryable();
            nestado_proc = "Estado : Sin Proceso";
            foreach (var item in procesoslist)
            {
                nestado_proc = item.nestado.Trim() + " Resol: " + item.resolucion.Trim() + " Del : " +
                               FechaTexto(item.fecha_mdto);
            }

            StateHasChanged();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error en getresoluciones: {ex.Message}");
        }
    }

    private async Task getprop()
    {
        try
        {
            await ShowLoading();
            propietlist = await propietariosService.PropietariosBypredial(comp_obj.codigo);
            dataprop = propietlist.AsQueryable();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error en getprop: {ex.Message}");
        }
    }

    async Task ShowLoading()
    {
        isLoading = true;
        await Task.Yield();
        isLoading = false;
    }
    
    private async Task LoadFiles(InputFileChangeEventArgs e)
    {
        try
        {
            errors.Clear();
            loadedFiles.Clear();
            Docsobj.Ruta = string.Empty;

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

            if (file.Size <= 0 || file.Size > maxFileSize)
            {
                errors.Add("El PDF debe pesar más de 0 y hasta 4 MB.");
                return;
            }

            var originalFileName = Path.GetFileNameWithoutExtension(file.Name);
            Docsobj.Ruta = $"{nombreArchivo}_{originalFileName}.pdf";
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

        if (string.IsNullOrWhiteSpace(Docsobj.Nombre))
        {
            errors.Add("Ingrese el detalle del documento.");
        }

        if (loadedFiles.Count != 1
            || string.IsNullOrWhiteSpace(Docsobj.Ruta)
            || !string.Equals(Path.GetExtension(Docsobj.Ruta), ".pdf", StringComparison.OrdinalIgnoreCase))
        {
            errors.Add("Seleccione un solo archivo con extensión .pdf.");
        }
        else if (loadedFiles[0].Size <= 0 || loadedFiles[0].Size > maxFileSize)
        {
            errors.Add("El PDF debe pesar más de 0 y hasta 4 MB.");
        }

        return errors.Count == 0;
    }
    
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
}