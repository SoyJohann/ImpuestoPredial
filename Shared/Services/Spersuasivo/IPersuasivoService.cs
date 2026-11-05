using Coactivos_Predial.Shared.Services.Common;
using Coactivos_Predial.Shared.Services.Spersuasivo.Responses;

namespace Coactivos_Predial.Shared.Services.Spersuasivo;

public interface IPersuasivoService
{
    Task<Result<List<PersuasivoDto>>> GetList(int opcion, string pcomp, DateTime pdesde, DateTime phasta, int pvigencia, int ptran);

    Task<Result> Procesa_Per(int opcion, string pcomp, DateTime pdesde, DateTime phasta, int pvigencia, int ptran);
}
