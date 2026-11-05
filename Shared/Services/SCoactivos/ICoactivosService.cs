using Coactivos_Predial.Shared.Services.Common;
using Coactivos_Predial.Shared.Services.SCoactivos.Responses;

namespace Coactivos_Predial.Shared.Services.SCoactivos;

public interface ICoactivosService
{
    Task<Result<List<CoactivoDto>>> GetList(int opcion, string pcomp, DateTime pdesde, DateTime phasta, int pvigencia, int ptran);

    Task<Result<List<CoactivoDto>>> GetList1(string x);

    Task<Result> Procesa_Coactivo(int opcion, string pcomp, DateTime pdesde, DateTime phasta, int pvigencia, int ptran);

    Task<Result<List<CoactivoDto>>> Lista_Coactivos(int opcion, string pcomp, int pvigencia, int ptran, int ptipo);

    Task<Result<List<TransaccionReporteDto>>> GetListtran(string ptipo, int pclase);
}
