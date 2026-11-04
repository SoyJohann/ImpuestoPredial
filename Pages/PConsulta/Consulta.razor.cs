using System.Security.Claims;
using Coactivos_Predial.Models;
using Coactivos_Predial.Shared.Models;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;

namespace Coactivos_Predial.Pages.PConsulta;

public partial class Consulta
{
    public Predios comp_obj { get; set; } = new Predios { };
    public List<Procesos> procesoslist { get; set; } = new List<Procesos>();
    public List<Propietarios> propietlist { get; set; } = new List<Propietarios>();
    private List<Documentos> listDocs = new List<Documentos>();
    private Documentos Docsobj = new Documentos();
    public List<Archivo> archivo = new();
    private List<IBrowserFile> loadedFiles = new();

    private long maxFileSize = 1024 * 1024 * 4;
    private int MaxAllowedFiles = 1;
    private List<string> errors = new();
    public string nombreArchivo = "NumeroComparendo";
    public string rutaparaarchivo;

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


    private async Task GetPdfs()
    {
        try
        {
            listDocs = await comparendoService.GetPdfsByComp(comp_obj.codigo);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al obtener Conceptos:{ex}");
        }
    }


    private async Task GetRuta_Pdf(InputFileChangeEventArgs e)
    {
        if (e != null && e.File != null)
        {
            var archivo = e.File;
            Console.WriteLine($"Error in obtenerLiquidacion: {archivo.Name}");
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
        await comparendoService.DeleteDocs(docs.id);

        // Eliminar el archivo físico
        try
        {
            if (File.Exists(docs.archivo))
            {
                File.Delete(docs.archivo);
                Console.WriteLine($"Archivo {docs.archivo} eliminado correctamente.");
            }
            else
            {
                Console.WriteLine($"El archivo {docs.archivo} no existe.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al eliminar el archivo {docs.archivo}: {ex.Message}");
        }

        docPendienteEliminar = null;
        mostrarConfirmacionEliminar = false;

        // Actualizar la lista de documentos
        await GetPdfs();
        StateHasChanged();
    }


    private async Task Agregar_Pdf()
    {
        if (string.IsNullOrEmpty(Docsobj.nombre))
        {
            await JSRuntime.InvokeVoidAsync("alert", "Por favor, ingresa el detalle del documento.");
            return;
        }
        if (string.IsNullOrEmpty(Docsobj.archivo))
        {
            await JSRuntime.InvokeVoidAsync("alert", "Por favor, escoja un documento a cargar.");
            return;
        }

        try
        {
            var count = await comparendoService.Add_Docs(Docsobj);

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
                string fullFilePath = Path.Combine(directoryPath, Docsobj.archivo);

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


    private async Task obtenercompare()
    {
        try
        {
            string result = busquedaService.ObtenerUltimoComparendoCapturado();
            comp_obj.codigo = result;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al asignar comparendo: {ex.Message}");
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

    private async Task ComparendoSeleccionadoDesdeModal(string codigoCapturado)
    {
        comp_obj.codigo = codigoCapturado;
        mostrarModalBusqueda = false;

        await obtenerLiquidacion();
        await getprop();
        await getprocesos();
    }


    private async Task CerrarModalb()
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
                this.tab1 = true; this.tab2 = false; this.tab3 = false;
                break;
            case 2:
                this.tab1 = false; this.tab2 = true; this.tab3 = false;
                break;
            case 3:
                this.tab1 = false; this.tab3 = true; this.tab2 = false;
                break;
        }
    }

    private async Task obtenerLiquidacion()
    {
        try
        {
            if (string.IsNullOrEmpty(comp_obj.codigo))
            {
                return;
            }
            var result = await comparendoService.GetMultasByComp(comp_obj.codigo.Trim());
            if (result != null)
            {
                comp_obj = result;
                if (comp_obj.exento)
                { nentidad = "Exento"; }
                else { nentidad = "Normal"; }
//                if (comp_obj.estado == 1)
  //              { nestado = "Pendiente Pago"; }
    //            else { nestado = "Cancelado"; }
                getprop(); existe = false;
                await GetPdfs();
                ptotalconrecFormated = (comp_obj.avaluo).ToString("C");
                displayTabs(1);
            }
            else
            {
                await GetPdfs();
                existe = true;
                comp_obj.nombre = ""; comp_obj.cedula = "";
                comp_obj.area_tet = 0;
                comp_obj.area_con = 0;
                comp_obj.dir = ""; comp_obj.matricula = ""; comp_obj.zona = ""; comp_obj.estrato = "";
                comp_obj.avaluo = 0;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in obtenerLiquidacion: {ex.Message}");
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
                nestado_proc = item.nestado.Trim() + " Resol: " + item.resolucion.Trim() + " Del : " + FechaTexto(item.fecha_mdto);
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

    private void MostrarModal()
    {
        Docsobj = new Documentos();
        Docsobj.codigo_predial = comp_obj.codigo;
        nombreArchivo = comp_obj.codigo;
        mostrarModal = true;
        StateHasChanged();
    }

    private void CerrarModal()
    {
        mostrarModal = false;
    }

    private async Task LoadFiles(InputFileChangeEventArgs e)
    {
        try
        {
            errors.Clear();
            if (e.FileCount > MaxAllowedFiles)
            {
                errors.Add($"Supera la cantidad permitida: {MaxAllowedFiles}");
                return;
            }
            loadedFiles.Clear();
            foreach (var file in e.GetMultipleFiles(MaxAllowedFiles))
            {
                try
                {
                    string originalFileName = Path.GetFileNameWithoutExtension(file.Name);
                    Console.WriteLine($"Original File Name: {originalFileName}");

                    string extension = Path.GetExtension(file.Name);
                    Console.WriteLine($"Extension: {extension}");

                    string newFileName = $"{nombreArchivo}_{originalFileName}{extension}";
                    Console.WriteLine($"New File Name: {newFileName}");

                    Docsobj.archivo = newFileName;

                    loadedFiles.Add(file);
                    Console.WriteLine($"File {file.Name} added to loadedFiles.");
                }
                catch (Exception ex)
                {
                    errors.Add($"{file.Name} error: {ex.Message}");
                    Console.WriteLine($"Error processing file {file.Name}: {ex.Message}");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Unexpected error in LoadFiles: {ex.Message}");
            throw;
        }
    }

    public async Task GetArchivos()
    {
        try
        {
            Console.WriteLine($"el nombre del archivo{nombreArchivo}");
            var result = await _archivoService.GetFiles(nombreArchivo);

            if (result == null)
            {
                Console.WriteLine("No encontrado");
            }
            else
            {
                archivo = result;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    public async Task Muestra_Pdf(string xfile)
    {
        try
        {
            var result = await _archivoService.GetFiles(xfile);

            Console.WriteLine($"xfile en getarchivos:{xfile}");
            if (result == null || result.Count == 0)
            {
                Console.WriteLine("No se encontraron archivos.");
            }
            else
            {
                foreach (var item in result)
                {
                    await DescargarYAbrirArchivo(item.Name);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    private async Task DescargarYAbrirArchivo(string fileName)
    {
        try
        {
            string fileUrl = $"{NavigationManager.BaseUri}api/archivos/{fileName}";

            var fileBytes = await DescargarArchivo(fileUrl);

            if (fileBytes != null)
            {
                var contentStream = new DotNetStreamReference(new MemoryStream(fileBytes));

                await JSRuntime.InvokeVoidAsync("downloadFileFromStream", fileName, contentStream);

                Console.WriteLine($"Archivo {fileName} descargado y abierto correctamente.");
            }
            else
            {
                Console.WriteLine($"Error: No se pudo descargar el archivo {fileName}.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al descargar y abrir el archivo: {ex.Message}");
        }
    }

    private async Task<byte[]> DescargarArchivo(string fileUrl)
    {
        try
        {
            Console.WriteLine($"Intentando descargar desde {fileUrl}");

            HttpResponseMessage response = await _httpClient.GetAsync(fileUrl);
            response.EnsureSuccessStatusCode();
            byte[] archivoBytes = await response.Content.ReadAsByteArrayAsync();

            Console.WriteLine($"Archivo descargado correctamente desde {fileUrl}");
            return archivoBytes;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al descargar el archivo: {ex.Message}");
            return null;
        }
    }
}