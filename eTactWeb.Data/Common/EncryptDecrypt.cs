using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace eTactWeb.Data.Common
{
    public class EncryptDecrypt
    {
        private readonly IConfiguration configuration;

        public EncryptDecrypt(IConfiguration config)
        {
            configuration = config;
        }

        //change from history to solve error by MONIKA
        public string Decrypt(string DecryptText)
        {
            var SrctArray = Encoding.UTF8.GetBytes(configuration["Cipher:Key"]);
            var DrctArray = Convert.FromBase64String(DecryptText);
            var objt = new TripleDESCryptoServiceProvider();
            var objmdcript = new MD5CryptoServiceProvider();
            SrctArray = objmdcript.ComputeHash(Encoding.UTF8.GetBytes(configuration["Cipher:Key"]));
            objmdcript.Clear();
            objt.Key = SrctArray;
            objt.Mode = CipherMode.ECB;
            objt.Padding = PaddingMode.PKCS7;
            var crptotrns = objt.CreateDecryptor();
            var resArray = crptotrns.TransformFinalBlock(DrctArray, 0, DrctArray.Length);
            objt.Clear();
            return Encoding.UTF8.GetString(resArray);
        }


        public string Encrypt(string Encryptval)
        {
            var SrctArray = Encoding.UTF8.GetBytes(configuration["Cipher:Key"]);
            var EnctArray = Encoding.UTF8.GetBytes(Encryptval);
            var objt = new TripleDESCryptoServiceProvider();
            var objcrpt = new MD5CryptoServiceProvider();
            SrctArray = objcrpt.ComputeHash(Encoding.UTF8.GetBytes(configuration["Cipher:Key"]));
            objcrpt.Clear();
            objt.Key = SrctArray;
            objt.Mode = CipherMode.ECB;
            objt.Padding = PaddingMode.PKCS7;
            var crptotrns = objt.CreateEncryptor();
            var resArray = crptotrns.TransformFinalBlock(EnctArray, 0, EnctArray.Length);
            objt.Clear();
            return Convert.ToBase64String(resArray, 0, resArray.Length);
        }
        public static string EncodeID(int id)
        {
            int obfuscated = id * 12345 + 6789; // simple obfuscation
            var bytes = BitConverter.GetBytes(obfuscated);
            string base64 = Convert.ToBase64String(bytes);
            return base64.TrimEnd('=').Replace('+', '-').Replace('/', '_');
        }

        // Decode URL-safe Base64 back to int
        public static int DecodeID(string encoded)
        {
            string padded = encoded.Replace('-', '+').Replace('_', '/');
            switch (padded.Length % 4)
            {
                case 2: padded += "=="; break;
                case 3: padded += "="; break;
            }
            var bytes = Convert.FromBase64String(padded);
            int obfuscated = BitConverter.ToInt32(bytes, 0);
            return (obfuscated - 6789) / 12345;
        }
    }
}