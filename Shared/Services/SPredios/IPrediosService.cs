using Coactivos_Predial.Shared.Models;
using Coactivos_Predial.Shared.Services.Common;
using Coactivos_Predial.Shared.Services.SPredios.Responses;

namespace Coactivos_Predial.Shared.Services.SPredios;

public interface IPrediosService
{
    Task<Result> AgregarDocumento(Documentos documento);

    Task<Result> DeleteDocs(int id);

    Task<Result<List<PredioBusquedaDto>>> BuscarPrediosPorCedula(string cedula);

    Task<Result<PredioDto?>> GetPrediosByCode(string codigoBusqueda);

    Task<Result<List<DocumentoDto>>> GetPdfsByCode(int predioId);

    Task<Result<List<ProcesoDto>>> GetCoactivo(string comp);

    Task<Result<List<DocumentoDiarioDto>>> Lista_Diario(DateTime pdesde, DateTime phasta);

    Task<Result<ResumenPdfsDto>> Pdfs_Resumen(DateTime pdesde, DateTime phasta);
}
