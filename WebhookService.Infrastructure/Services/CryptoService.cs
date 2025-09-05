using System.Security.Cryptography;
using System.Text;
using WebhookService.Appliaction.Contract;

namespace WebhookService.Infrastructure.Services
{
    public class CryptoService(string masterKey) : ICryptoService
    {
        private readonly byte[] _key = SHA256.HashData(Encoding.UTF8.GetBytes(masterKey));

        public string Encrypt(string plainText)
        {
            using var aes = Aes.Create();
            aes.Key = _key;
            aes.GenerateIV();

            var encryptor = aes.CreateEncryptor();
            var encrypted = encryptor.TransformFinalBlock(
                Encoding.UTF8.GetBytes(plainText), 0, plainText.Length);

            var result = new byte[aes.IV.Length + encrypted.Length];
            Array.Copy(aes.IV, 0, result, 0, aes.IV.Length);
            Array.Copy(encrypted, 0, result, aes.IV.Length, encrypted.Length);

            return Convert.ToBase64String(result);
        }

        public string Decrypt(string cipherText)
        {
            var buffer = Convert.FromBase64String(cipherText);
            using var aes = Aes.Create();
            aes.Key = _key;

            var iv = new byte[16];
            var cipher = new byte[buffer.Length - 16];

            Array.Copy(buffer, 0, iv, 0, 16);
            Array.Copy(buffer, 16, cipher, 0, cipher.Length);

            aes.IV = iv;
            var decryptor = aes.CreateDecryptor();
            var decrypted = decryptor.TransformFinalBlock(cipher, 0, cipher.Length);

            return Encoding.UTF8.GetString(decrypted);
        }

        public string GenerateSecret()
        {
            var bytes = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(bytes);
            return Convert.ToBase64String(bytes);
        }

        public string ComputeSignature(string secret, string payload)
        {
            using var hmac = new HMACSHA256(Convert.FromBase64String(secret));
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(payload));
            return Convert.ToBase64String(hash);
        }
    }
}