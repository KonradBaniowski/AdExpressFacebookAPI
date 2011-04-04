using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Specialized;
using System.Security.Cryptography;
using System.IO;

namespace TNS.AdExpress.Web.Functions
{

    /// <summary>
    /// Query String Helper
    /// </summary>
    public class QueryStringEncryption
    {

        public const string _cryptKey = "8!b?#B$3";

        public static string EncryptQueryString(string strQueryString)
        {
            Encryption64 oES =
                new Encryption64();
            return oES.Encrypt(strQueryString, _cryptKey).Replace("+", "-").Replace("/", "_");
        }

        public static string DecryptQueryString(string strQueryString)
        {
            Encryption64 oES =
                new Encryption64();
            return oES.Decrypt(strQueryString.Replace("-", "+").Replace("_", "/"), _cryptKey);
        }



    }

    public class Encryption64
    {
        private Byte[] IV = new byte[] { 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF };

        public string Decrypt(string stringToDecrypt, string sEncryptionKey)
        {
            byte[] inputByteArray = new byte[(stringToDecrypt.Length)];
            byte[] key = System.Text.Encoding.UTF8.GetBytes(sEncryptionKey);
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            inputByteArray = Convert.FromBase64String(stringToDecrypt);
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(key, IV), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();
            System.Text.Encoding encoding = System.Text.Encoding.UTF8;
            return encoding.GetString(ms.ToArray());
        }

        public string Encrypt(string stringToEncrypt, String SEncryptionKey)
        {
            byte[] key = System.Text.Encoding.UTF8.GetBytes(SEncryptionKey);
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            byte[] inputByteArray = Encoding.UTF8.GetBytes(stringToEncrypt);
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(key, IV), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();
            return Convert.ToBase64String(ms.ToArray());
        }

    }
}
