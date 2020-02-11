using System;
using System.Linq;
using System.Security.Cryptography;

namespace Moneymo.AuthenticationService.Core.Helpers
{
    internal static class HashHelper
    {
        private static int _saltByteLength = 8;
        private static int _derivedKeyLength = 32;
        private static int _iterationCount = 100;

        internal static (string passwordHash, string salt) GeneratePasswordHash(string password)
        {
            var salt = GenerateRandomSaltStr();
            var passwordHash = Convert.ToBase64String(GenerateHashValue(password, Convert.FromBase64String(salt)));

            return (passwordHash, salt);
        }

        public static bool Verify(string attemptedPassword, string encryptedPassword, string salt)
        {
            return Verify(attemptedPassword, Convert.FromBase64String(encryptedPassword), Convert.FromBase64String(salt));
        }

        private static byte[] GenerateHashValue(string password, byte[] salt)
        {
            byte[] hashValue;
            var valueToHash = string.IsNullOrEmpty(password) ? string.Empty : password;
            using (var pbkdf2 = new Rfc2898DeriveBytes(valueToHash, salt, _iterationCount))
            {
                hashValue = pbkdf2.GetBytes(_derivedKeyLength);
            }
            return hashValue;
        }

        private static bool Verify(string attemptedPassword, byte[] encryptedPassword, byte[] salt)
        {
            byte[] encryptedAttPwd = GenerateHashValue(attemptedPassword, salt);
            var passHash = Convert.ToBase64String(encryptedAttPwd);
            return encryptedPassword.SequenceEqual<byte>(encryptedAttPwd);
        }

        private static byte[] GenerateRandomSalt()
        {
            var csprng = new RNGCryptoServiceProvider();
            var salt = new byte[_saltByteLength];
            csprng.GetBytes(salt);
            return salt;
        }
        private static string GenerateRandomSaltStr()
        {
            return Convert.ToBase64String(GenerateRandomSalt());
        }
    }
}