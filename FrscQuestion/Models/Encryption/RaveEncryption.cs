using System;
using System.Security.Cryptography;
using System.Text;

namespace FrscQuestion.Models.Encryption
{
    public class RaveEncryption
    {
        /// <summary>
        ///     Gets an encryption key from rave secret key.
        /// </summary>
        /// <param name="secretKey">The secret key generated from your rave dashboard</param>
        /// <returns>a string value encrypted</returns>
        public string GetEncryptionKey(string secretKey)
        {
            //MD5 is the hash algorithm expected by rave to generate encryption key
            var md5 = new MD5CryptoServiceProvider();
            //MD5CryptoServiceProvider works with bytes so a conversion of plain secretKey to it bytes equivalent is required.
            //UTF8Encoding.UTF8.GetBytes(secretKey) can also be used.
            var secretKeyBytes = Encoding.UTF8.GetBytes(secretKey);


            var hashedSecret = md5.ComputeHash(secretKeyBytes, 0, secretKeyBytes.Length);
            var hashedSecretLast12Bytes = new byte[12];
            Array.Copy(hashedSecret, hashedSecret.Length - 12, hashedSecretLast12Bytes, 0, 12);
            var hashedSecretLast12HexString = BitConverter.ToString(hashedSecretLast12Bytes);
            hashedSecretLast12HexString = hashedSecretLast12HexString.ToLower().Replace("-", "");
            var secretKeyFirst12 = secretKey.Replace("FLWSECK-", "").Substring(0, 12);
            var hashedSecretLast12HexBytes = Encoding.UTF8.GetBytes(hashedSecretLast12HexString);
            var secretFirst12Bytes = Encoding.UTF8.GetBytes(secretKeyFirst12);
            var combineKey = new byte[24];
            Array.Copy(secretFirst12Bytes, 0, combineKey, 0, secretFirst12Bytes.Length);
            Array.Copy(hashedSecretLast12HexBytes, hashedSecretLast12HexBytes.Length - 12, combineKey, 12, 12);
            return Encoding.UTF8.GetString(combineKey);
        }

        // This is the encryption function that encrypts your payload by passing the stringified format and your encryption Key.
        public string EncryptData(string encryptionKey, string data)
        {
            TripleDES des = new TripleDESCryptoServiceProvider();
            des.Mode = CipherMode.ECB;
            des.Padding = PaddingMode.PKCS7;
            des.Key = Encoding.UTF8.GetBytes(encryptionKey);
            var cryptoTransform = des.CreateEncryptor();
            var dataBytes = Encoding.UTF8.GetBytes(data);
            var encryptedDataBytes = cryptoTransform.TransformFinalBlock(dataBytes, 0, dataBytes.Length);
            des.Clear();
            return Convert.ToBase64String(encryptedDataBytes);
        }

        public string DecryptData(string encryptedData, string encryptionKey)
        {
            var des = new TripleDESCryptoServiceProvider();
            des.Key = Encoding.UTF8.GetBytes(encryptionKey);
            des.Mode = CipherMode.ECB;
            des.Padding = PaddingMode.PKCS7;
            var cryptoTransform = des.CreateDecryptor();
            var EncryptDataBytes = Convert.FromBase64String(encryptedData);
            var plainDataBytes = cryptoTransform.TransformFinalBlock(EncryptDataBytes, 0, EncryptDataBytes.Length);
            des.Clear();
            return Encoding.UTF8.GetString(plainDataBytes);
        }
    }
}