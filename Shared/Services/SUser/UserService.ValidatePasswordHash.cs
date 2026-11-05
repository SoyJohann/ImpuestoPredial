using System.Security.Cryptography;

namespace Coactivos_Predial.Shared.Services.SUser;

public partial class UserService
{
    public bool ValidatePasswordHash(string password, string dbPassword)
    {
        byte[] dbPasswordHashBytes = Convert.FromBase64String(dbPassword);
        byte[] salt = new byte[16];
        Array.Copy(dbPasswordHashBytes, 0, salt, 0, 16);

        var userPasswordBytes = new Rfc2898DeriveBytes(password, salt, 1000);
        byte[] userPasswordHash = userPasswordBytes.GetBytes(20);

        for (int i = 0; i < 20; i++)
        {
            if (dbPasswordHashBytes[i + 16] != userPasswordHash[i])
            {
                return false;
            }
        }

        return true;
    }
}
