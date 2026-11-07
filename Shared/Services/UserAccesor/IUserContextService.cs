namespace Coactivos_Predial.Shared.Services.UserAccesor
{
    public interface IUserContextService
    {
        string? GetUserMunicipio();

        /// <summary>Identificador del usuario autenticado tomado de la cookie.</summary>
        Task<int> GetUserIdAsync();

        /// <summary>Nombre del usuario autenticado tomado de la cookie.</summary>
        Task<string?> GetUserNameAsync();
    }
}
