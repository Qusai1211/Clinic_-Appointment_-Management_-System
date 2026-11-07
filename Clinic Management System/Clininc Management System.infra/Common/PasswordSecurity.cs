using System.Security.Cryptography;
using System.Text;

namespace Clininc_Management_System.infra.Common
{
    public static class PasswordSecurity
    {
        private const int SaltByteSize = 16;
        private const int HashByteSize = 32;
        private const int Iterations = 100000;
        private static readonly HashAlgorithmName Algorithm = HashAlgorithmName.SHA256;

        /// <summary>
        /// Hashes a password using PBKDF2 (HMAC-SHA256, 100k iterations, random 128-bit salt).
        /// Format: PBKDF2$100000$&lt;salt_b64&gt;$&lt;hash_b64&gt;
        /// </summary>
        public static string HashPassword(string password)
        {
            byte[] salt = RandomNumberGenerator.GetBytes(SaltByteSize);
            byte[] hash = Rfc2898DeriveBytes.Pbkdf2(
                Encoding.UTF8.GetBytes(password),
                salt,
                Iterations,
                Algorithm,
                HashByteSize
            );

            return $"PBKDF2${Iterations}${Convert.ToBase64String(salt)}${Convert.ToBase64String(hash)}";
        }

        /// <summary>
        /// Verifies a password against a stored hash.
        /// Supports PBKDF2 hashes and legacy SHA-256 hashes (setting needsRehash = true for transparent upgrade).
        /// </summary>
        public static bool VerifyPassword(string password, string storedHash, out bool needsRehash)
        {
            needsRehash = false;
            if (string.IsNullOrWhiteSpace(storedHash) || string.IsNullOrWhiteSpace(password))
            {
                return false;
            }

            // Check if stored hash is PBKDF2 format
            if (storedHash.StartsWith("PBKDF2$", StringComparison.OrdinalIgnoreCase))
            {
                var parts = storedHash.Split('$');
                if (parts.Length == 4 && int.TryParse(parts[1], out int iter))
                {
                    try
                    {
                        byte[] salt = Convert.FromBase64String(parts[2]);
                        byte[] expectedHash = Convert.FromBase64String(parts[3]);

                        byte[] actualHash = Rfc2898DeriveBytes.Pbkdf2(
                            Encoding.UTF8.GetBytes(password),
                            salt,
                            iter,
                            Algorithm,
                            expectedHash.Length
                        );

                        return CryptographicOperations.FixedTimeEquals(actualHash, expectedHash);
                    }
                    catch
                    {
                        return false;
                    }
                }
            }

            // Legacy SHA-256 fallback migration path
            using (var sha = SHA256.Create())
            {
                var computedBytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
                var hex = Convert.ToHexString(computedBytes);

                if (string.Equals(hex, storedHash, StringComparison.OrdinalIgnoreCase))
                {
                    needsRehash = true; // Signals caller to upgrade stored hash to PBKDF2
                    return true;
                }
            }

            return false;
        }
    }
}
