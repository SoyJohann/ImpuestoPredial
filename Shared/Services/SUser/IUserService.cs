using Coactivos_Predial.Shared.Services.Common;
using Coactivos_Predial.Shared.Services.SUser.Responses;

namespace Coactivos_Predial.Shared.Services.SUser;

public interface IUserService
{
    Task<Result> Add(UsuarioDto usuarios);

    Task<Result> Change_key(int pid, string skey);

    Task<Result<List<UsuarioDto>>> GetAll();

    Task<Result> Reset_key(int pid);

    Task<Result> EditUsuarios(UsuarioDto usuarios);

    Task<Result> DeleteUsuarioById(int id);

    Task<Result<UsuarioDto?>> GetUserById(int id);

    Task<Result<List<UsuarioDto>>> GetUserByName(string nombre);

    bool ValidatePasswordHash(string password, string dbPassword);
}
