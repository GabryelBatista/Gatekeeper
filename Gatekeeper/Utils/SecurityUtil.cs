using System.Security.Cryptography;

namespace Gatekeeper.Utils;

public static class SecurityUtil
{
    public static string HashPassword(string password)
    {
        var saltBytes = new byte[32];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(saltBytes);
        }

        using (var pbkdf2 = new Rfc2898DeriveBytes(password, saltBytes, 10000))
        {
            var hashBytes = pbkdf2.GetBytes(20);
            var hash = new byte[52];
            Array.Copy(saltBytes, 0, hash, 0, 32);
            Array.Copy(hashBytes, 0, hash, 32, 20);
            return Convert.ToBase64String(hash);
        }
    }
    
    public static bool VerifyPassword(string password, string hashedPassword)
    {
        var hashBytes = Convert.FromBase64String(hashedPassword);
        var saltBytes = new byte[32];
        Array.Copy(hashBytes, 0, saltBytes, 0, 32);
        using (var pbkdf2 = new Rfc2898DeriveBytes(password, saltBytes, 10000))
        {
            var hash = pbkdf2.GetBytes(20);
            for (var i = 0; i < 20; i++)
            {
                if (hashBytes[i + 32] != hash[i])
                {
                    return false;
                }
            }
        }

        return true;
    }
}