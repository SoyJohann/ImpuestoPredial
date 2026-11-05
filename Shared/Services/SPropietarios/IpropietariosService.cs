using Coactivos_Predial.Shared.Services.Common;
using Coactivos_Predial.Shared.Services.SPropietarios.Responses;

namespace Coactivos_Predial.Shared.Services.SPropietarios;

public interface IpropietariosService
{
    Task<Result> Add(PropietarioDto infractor);

    Task<Result<List<PropietarioDto>>> GetInfractoresByCedula(string cedula);

    Task<Result<List<PropietarioDto>>> PropietariosBypredial(string codigo);

    Task<Result<string?>> GetNameInfractorBycode(string code, int tipo);

    Task<Result<List<PropietarioDto>>> GetList();

    Task<Result<PropietarioDto?>> GetInfractorById(int id);

    Task<Result<List<PropietarioDto>>> GetAll();

    Task<Result> EditInfractores(PropietarioDto infractor);

    Task<Result> DeleteInfractores(int id);

    Task<Result<PropietarioDto?>> GetInfractores(int id);
}
