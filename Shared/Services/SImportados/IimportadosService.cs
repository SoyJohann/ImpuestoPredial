using Coactivos_Predial.Models;
using Coactivos_Predial.Shared.Models;

namespace Coactivos_Predial.Shared.Services.SImportados
{
    public interface IimportadosService
    {
        Task<int> Add(Importados import_obj);

        Task<List<Importados>> GetList();

        Task<int> DeleteAll();
        Task<int> DeleteRec();
        Task<int> Procesa_Comp(int puser);
        
        Task<int> Procesa_Rec(int puser);
    }
}
