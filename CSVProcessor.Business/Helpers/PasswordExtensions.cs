using System;
using System.Security.Cryptography;

namespace CSVProcessor.Business.Helpers
{
    public static class PasswordExtensions
    {
        public static string[] HasAndSaltIt(this string password)
        {
            var serviceProvider = new RNGCryptoServiceProvider();
            var salt = new byte[SALT_BYTES];
            serviceProvider.GetBytes(salt);
            var hash = Pbkdf2(password, salt, PBKDF2_ITERATIONS, HASH_BYTES);

            var hashAndSalt = new string[2];
            hashAndSalt[0] = Convert.ToBase64String(hash);
            hashAndSalt[1] = Convert.ToBase64String(salt);
            return hashAndSalt;
        }

        //

        private const int SALT_BYTES = 24;
        private const int HASH_BYTES = 24;
        private const int PBKDF2_ITERATIONS = 1000;


        private static byte[] Pbkdf2(string password, byte[] salt, int iterations, int outputBytes)
        {
            var pbkdf2 = new Rfc2898DeriveBytes(password, salt) { IterationCount = iterations };
            return pbkdf2.GetBytes(outputBytes);
        }

    }
}
