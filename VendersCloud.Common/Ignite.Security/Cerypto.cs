using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace IgniteSecurityLib
{
    public abstract class OperationResult
    {
        public bool Success { get; set; }
        public string ExceptionMessage { get; set; }
    }

    public class AsymmetricKeyPairGenerationResult : OperationResult
    {
        public string PublicKeyPem { get; set; }
        public string PrivateKeyPem { get; set; }
    }

    public class AsymmetricEncryptionService
    {
        public AsymmetricKeyPairGenerationResult GenerateKeys(int keySizeBits)
        {
            var result = new AsymmetricKeyPairGenerationResult();

            try
            {
                using var rsa = RSA.Create(keySizeBits);
                result.PublicKeyPem = ToPem(rsa.ExportSubjectPublicKeyInfo());
                result.PrivateKeyPem = ToPem(rsa.ExportPkcs8PrivateKey());
                result.Success = true;
            }
            catch (CryptographicException ex)
            {
                result.ExceptionMessage = $"Error generating RSA keys: {ex.Message}";
            }

            return result;
        }

        public AsymmetricEncryptionResult EncryptWithPublicKey(string message, string publicKeyPem)
        {
            var result = new AsymmetricEncryptionResult();

            try
            {
                using var rsa = RSA.Create();
                rsa.ImportFromPem(publicKeyPem);

                var encryptedBytes = rsa.Encrypt(Encoding.UTF8.GetBytes(message), RSAEncryptionPadding.OaepSHA256);
                result.EncryptedAsBytes = encryptedBytes;
                result.EncryptedAsBase64 = System.Convert.ToBase64String(encryptedBytes);
                result.Success = true;
            }
            catch (Exception ex)
            {
                result.ExceptionMessage = $"Encryption failed: {ex.Message}";
            }

            return result;
        }

        public AsymmetricDecryptionResult DecryptWithPrivateKey(byte[] cipherBytes, string privateKeyPem)
        {
            var result = new AsymmetricDecryptionResult();

            try
            {
                using var rsa = RSA.Create();
                rsa.ImportFromPem(privateKeyPem);

                var decryptedBytes = rsa.Decrypt(cipherBytes, RSAEncryptionPadding.OaepSHA256);
                result.DecryptedMessage = Encoding.UTF8.GetString(decryptedBytes);
                result.Success = true;
            }
            catch (Exception ex)
            {
                result.ExceptionMessage = $"Decryption failed: {ex.Message}";
            }

            return result;
        }

        public static string ToPem(byte[] data)
        {
            return "-----BEGIN KEY-----\n" +
                   System.Convert.ToBase64String(data, Base64FormattingOptions.InsertLineBreaks) +
                   "\n-----END KEY-----";
        }
    }

    public class AsymmetricEncryptionResult : OperationResult
    {
        public byte[] EncryptedAsBytes { get; set; }
        public string EncryptedAsBase64 { get; set; }
    }

    public class AsymmetricDecryptionResult : OperationResult
    {
        public string DecryptedMessage { get; set; }
    }

    public class SymmetricAesEncryptionService
    {
        public static string Encrypt(string secretKey, string plainText)
        {
            if (string.IsNullOrEmpty(plainText))
                throw new ArgumentNullException(nameof(plainText));

            using var aes = Aes.Create();
            aes.Key = GenerateKey(secretKey, aes.KeySize / 8);
            aes.GenerateIV();

            using var ms = new MemoryStream();
            ms.Write(BitConverter.GetBytes(aes.IV.Length));
            ms.Write(aes.IV);

            using (var cryptoStream = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
            using (var writer = new StreamWriter(cryptoStream))
            {
                writer.Write(plainText);
            }

            return System.Convert.ToBase64String(ms.ToArray());
        }

        public static string Decrypt(string secretKey, string cipherText)
        {
            if (string.IsNullOrEmpty(cipherText))
                throw new ArgumentNullException(nameof(cipherText));

            using var aes = Aes.Create();
            var cipherBytes = System.Convert.FromBase64String(cipherText);

            using var ms = new MemoryStream(cipherBytes);
            var ivLength = BitConverter.ToInt32(ms.ReadBytes(sizeof(int)));
            aes.IV = ms.ReadBytes(ivLength);
            aes.Key = GenerateKey(secretKey, aes.KeySize / 8);

            using var cryptoStream = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Read);
            using var reader = new StreamReader(cryptoStream);

            return reader.ReadToEnd();
        }

        private static byte[] GenerateKey(string secretKey, int size)
        {
            using var sha256 = SHA256.Create();
            return sha256.ComputeHash(Encoding.UTF8.GetBytes(secretKey)).Take(size).ToArray();
        }
    }

    public static class StreamExtensions
    {
        public static byte[] ReadBytes(this Stream stream, int count)
        {
            var buffer = new byte[count];
            var bytesRead = stream.Read(buffer, 0, count);
            if (bytesRead != count)
                throw new EndOfStreamException("Unexpected end of stream.");
            return buffer;
        }

      
    }
}
