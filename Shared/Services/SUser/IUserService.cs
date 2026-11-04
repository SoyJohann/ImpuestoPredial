using Coactivos_Predial.Models;
using Coactivos_Predial.Shared.Models;

namespace Coactivos_Predial.Shared.Services.SUser
{
    public interface IUserService
    {
        Task<int> Add(Usuarios usuarios);
        
        Task<int> Change_key(int pid, string skey);
        Task<List<Usuarios>> GetAll();
        Task<int> Reset_key(int pid);
        Task<int> EditUsuarios(Usuarios usuarios);

        Task<int> DeleteUsuarioById(int id);

        Task<Usuarios> GetUserById(int id);

        Task<List<Usuarios>> GetUserByName(string nombre);

        bool ValidatePasswordHash(string password, string dbPassword);
    }
}
