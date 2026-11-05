using System.Security.Cryptography;

namespace Coactivos_Predial.Shared.Services.SLogin;

public partial class LoginService
{
    public bool ValidatePasswordHash(string password, string storedHash)
    {
        if (string.IsNullOrEmpty(password) || string.IsNullOrWhiteSpace(storedHash))
            return false;

        try
        {
            var storedBytes = Convert.FromBase64String(storedHash);
            if (storedBytes.Length != 36)
                return false;

            var salt = storedBytes[..16];
            var expectedHash = storedBytes[16..];
            var actualHash = Rfc2898DeriveBytes.Pbkdf2(
                password,
                salt,
                1000,
                HashAlgorithmName.SHA1,
                expectedHash.Length);
            return CryptographicOperations.FixedTimeEquals(actualHash, expectedHash);
        }
        catch (FormatException)
        {
            return false;
        }
    }
}
