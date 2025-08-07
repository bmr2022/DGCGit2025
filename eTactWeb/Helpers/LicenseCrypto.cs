using System.Security.Cryptography;

namespace eTactWeb.Helpers
{
    public static class LicenseCrypto
    {
        // private static readonly string Key = "your-32-char-key-goes-here"; // Keep safe!
        private static readonly string Key = "0123456789ABCDEF0123456789ABCDEF";

        public static string Encrypt(string text)
        {
            using var aes = System.Security.Cryptography.Aes.Create();
            aes.Key = System.Text.Encoding.UTF8.GetBytes(Key);
            aes.GenerateIV();
            using var encryptor = aes.CreateEncryptor();
            using var ms = new MemoryStream();
            ms.Write(aes.IV, 0, 16);
            using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
            using (var sw = new StreamWriter(cs))
                sw.Write(text);
            return Convert.ToBase64String(ms.ToArray());
        }
        public static string Decrypt(string cipherText)
        {
            var combined = Convert.FromBase64String(cipherText);
            using var aes = System.Security.Cryptography.Aes.Create();
            aes.Key = System.Text.Encoding.UTF8.GetBytes(Key);
            var iv = new byte[16];
            Array.Copy(combined, iv, 16);
            aes.IV = iv;
            using var decryptor = aes.CreateDecryptor();
            using var ms = new MemoryStream(combined, 16, combined.Length - 16);
            using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
            using var sr = new StreamReader(cs);
            return sr.ReadToEnd();
        }
    }

}
