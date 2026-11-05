using Coactivos_Predial.Shared.Models;
using Coactivos_Predial.Shared.Services.Common;
using Coactivos_Predial.Shared.Services.SImportados.Responses;

namespace Coactivos_Predial.Shared.Services.SImportados;

public interface IimportadosService
{
    Task<Result> Add(Importados import_obj);

    Task<Result<List<ImportadoDto>>> GetList();

    Task<Result> DeleteAll();

    Task<Result> DeleteRec();

    Task<Result> Procesa_Comp(int puser);

    Task<Result> Procesa_Rec(int puser);
}
