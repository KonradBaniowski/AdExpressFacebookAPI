using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Web;

namespace Km.AdExpressClientWeb.Helpers
{
    public static class SecurityHelper
    {
        public const string CryptKey = "8!b?#B$3";

        private static Byte[] IV = new byte[] { 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF };

        public static string Decrypt(this string stringToDecrypt, string sEncryptionKey)
        {
            byte[] inputByteArray = new byte[(stringToDecrypt.Length)];
            byte[] key = System.Text.Encoding.UTF8.GetBytes(sEncryptionKey);
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            inputByteArray = HttpServerUtility.UrlTokenDecode(stringToDecrypt);
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(key, IV), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();
            System.Text.Encoding encoding = System.Text.Encoding.UTF8;
            return encoding.GetString(ms.ToArray());
        }

        public static string Encrypt(this string stringToEncrypt, String SEncryptionKey)
        {
            byte[] key = System.Text.Encoding.UTF8.GetBytes(SEncryptionKey);
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            byte[] inputByteArray = System.Text.Encoding.UTF8.GetBytes(stringToEncrypt);
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(key, IV), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();
            return HttpServerUtility.UrlTokenEncode(ms.ToArray());
        }
    }
}