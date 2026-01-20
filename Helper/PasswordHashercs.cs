using System.Security.Cryptography;

namespace ESSPMemberService.Helper
{

    public static class PasswordHasher
    {
        public static string HashPassword(string password)
        {
            using (var deriveBytes = new Rfc2898DeriveBytes(password, 16, 10000))
            {
                var salt = deriveBytes.Salt;
                var key = deriveBytes.GetBytes(32);

                var hashBytes = new byte[48];
                Buffer.BlockCopy(salt, 0, hashBytes, 0, 16);
                Buffer.BlockCopy(key, 0, hashBytes, 16, 32);

                return Convert.ToBase64String(hashBytes);
            }
        }

        public static bool VerifyPassword(string password, string storedHash)
        {
            var hashBytes = Convert.FromBase64String(storedHash);

            var salt = new byte[16];
            Buffer.BlockCopy(hashBytes, 0, salt, 0, 16);

            using (var deriveBytes = new Rfc2898DeriveBytes(password, salt, 10000))
            {
                var key = deriveBytes.GetBytes(32);

                for (int i = 0; i < 32; i++)
                    if (hashBytes[i + 16] != key[i])
                        return false;

                return true;
            }
        }
    }

}
