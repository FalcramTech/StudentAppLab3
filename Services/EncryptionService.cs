using System.Security.Cryptography;
using System.Text;

namespace StudentApp.Services
{
    public class EncryptionService
    {
        private const string KeyName = "StudentCardEncryptionKey";

        private async Task<byte[]> GetKeyAsync()
        {
            string? storedKey = await SecureStorage.Default.GetAsync(KeyName);

            if (storedKey != null)
                return Convert.FromBase64String(storedKey);

            byte[] key = RandomNumberGenerator.GetBytes(32);

            await SecureStorage.Default.SetAsync(
                KeyName,
                Convert.ToBase64String(key));

            return key;
        }

        public async Task<string> EncryptAsync(string text)
        {
            if (string.IsNullOrEmpty(text))
                return "";

            byte[] key = await GetKeyAsync();
            byte[] nonce = RandomNumberGenerator.GetBytes(12);
            byte[] plaintext = Encoding.UTF8.GetBytes(text);
            byte[] ciphertext = new byte[plaintext.Length];
            byte[] tag = new byte[16];

            using var aes = new AesGcm(key, 16);

            aes.Encrypt(
                nonce,
                plaintext,
                ciphertext,
                tag);

            byte[] result = new byte[
                nonce.Length + tag.Length + ciphertext.Length];

            Buffer.BlockCopy(nonce, 0, result, 0, nonce.Length);
            Buffer.BlockCopy(tag, 0, result, nonce.Length, tag.Length);
            Buffer.BlockCopy(
                ciphertext, 0, result,
                nonce.Length + tag.Length,
                ciphertext.Length);

            return Convert.ToBase64String(result);
        }

        public async Task<string> DecryptAsync(string encrypted)
        {
            if (string.IsNullOrEmpty(encrypted))
                return "";

            byte[] data = Convert.FromBase64String(encrypted);

            if (data.Length < 28)
                throw new CryptographicException("Некоректні зашифровані дані.");

            byte[] key = await GetKeyAsync();

            byte[] nonce = data[..12];
            byte[] tag = data[12..28];
            byte[] ciphertext = data[28..];
            byte[] plaintext = new byte[ciphertext.Length];

            using var aes = new AesGcm(key, 16);

            aes.Decrypt(
                nonce,
                ciphertext,
                tag,
                plaintext);

            return Encoding.UTF8.GetString(plaintext);
        }
    }
}
