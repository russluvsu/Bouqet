using System;
using System.Security.Cryptography;

namespace Bouqet
{
    /// <summary>
    /// Hashes and verifies passwords with PBKDF2-HMAC-SHA256.
    /// Stored format:  v1.{iterations}.{saltBase64}.{hashBase64}
    /// Everything needed to verify is inside the string, so the iteration
    /// count can be raised later without breaking existing accounts.
    /// Needs .NET Framework 4.7.2 or newer (the project targets 4.8).
    /// </summary>
    public static class PasswordHasher
    {
        private const string Version = "v1";
        private const int SaltSize = 16;
        private const int KeySize = 32;
        private const int DefaultIterations = 600000;

        // A real hash of a throwaway password. Used to spend the same time
        // when no account matches, so response time does not reveal which
        // emails exist.
        private static readonly string DummyHash = Hash("bouqet-timing-placeholder");

        public static string Hash(string password)
        {
            if (password == null) throw new ArgumentNullException("password");

            byte[] salt = new byte[SaltSize];
            using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            byte[] key;
            using (Rfc2898DeriveBytes kdf = new Rfc2898DeriveBytes(password, salt, DefaultIterations, HashAlgorithmName.SHA256))
            {
                key = kdf.GetBytes(KeySize);
            }

            return string.Join(".", Version, DefaultIterations.ToString(), Convert.ToBase64String(salt), Convert.ToBase64String(key));
        }

        public static bool Verify(string password, string stored)
        {
            if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(stored)) return false;

            string[] parts = stored.Split('.');
            if (parts.Length != 4 || parts[0] != Version) return false;

            int iterations;
            if (!int.TryParse(parts[1], out iterations) || iterations < 1) return false;

            byte[] salt;
            byte[] expected;
            try
            {
                salt = Convert.FromBase64String(parts[2]);
                expected = Convert.FromBase64String(parts[3]);
            }
            catch (FormatException)
            {
                return false;
            }

            byte[] actual;
            using (Rfc2898DeriveBytes kdf = new Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA256))
            {
                actual = kdf.GetBytes(expected.Length);
            }

            return ConstantTimeEquals(actual, expected);
        }

        /// <summary>Call when no account was found, to keep timing the same.</summary>
        public static void BurnTime(string password)
        {
            Verify(password, DummyHash);
        }

        // Compares every byte so the time taken does not depend on where
        // the first difference is.
        private static bool ConstantTimeEquals(byte[] a, byte[] b)
        {
            if (a.Length != b.Length) return false;

            int diff = 0;
            for (int i = 0; i < a.Length; i++)
            {
                diff |= a[i] ^ b[i];
            }
            return diff == 0;
        }
    }
}