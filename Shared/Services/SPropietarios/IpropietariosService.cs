using Coactivos_Predial.Models;
using Coactivos_Predial.Shared.Models;

namespace Coactivos_Predial.Shared.Services.SPropietarios
{
    public interface IpropietariosService
    {

        Task<int > Add(Propietarios infractores);

        Task<List<Propietarios>> GetInfractoresByCedula(string cedula);

        Task<List<Propietarios>> PropietariosBypredial(string codigo);
        Task<string> GetNameInfractorBycode(string code,int tipo);
        Task<List<Propietarios>> GetList();

        Task<Propietarios> GetInfractorById(int id);
        Task<List<Propietarios>> GetAll();
        
        Task<int> EditInfractores(Propietarios infracciones);
        
        int DeleteInfractores(int id);

        Propietarios GetInfractores(int id);


    }
}
