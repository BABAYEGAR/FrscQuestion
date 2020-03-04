using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace FrscQuestion.Models.Encryption
{
    public class Md5Ecryption
    {
        private static readonly byte[] bytes = Encoding.ASCII.GetBytes("ZeroCool");
        private readonly Random random = new Random();

        /// <summary>
        ///     This method converts a string to a MD5 hash algorith, string
        /// </summary>
        /// <param name="originalPassword"></param>
        /// <returns></returns>
        public string ConvertStringToMd5Hash(string originalPassword)
        {
            return string.Join("",
                MD5.Create().ComputeHash(Encoding.ASCII.GetBytes(originalPassword)).Select(s => s.ToString("x2")));
        }

        public string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public string Encrypt(string originalString)

        {
            if (string.IsNullOrEmpty(originalString))

            {
            }

            var cryptoProvider = new DESCryptoServiceProvider();

            var memoryStream = new MemoryStream();

            var cryptoStream = new CryptoStream(memoryStream,
                cryptoProvider.CreateEncryptor(bytes, bytes), CryptoStreamMode.Write);

            var writer = new StreamWriter(cryptoStream);

            writer.Write(originalString);

            writer.Flush();

            cryptoStream.FlushFinalBlock();

            writer.Flush();

            return Convert.ToBase64String(memoryStream.GetBuffer(), 0, (int) memoryStream.Length);
        }
    }
}