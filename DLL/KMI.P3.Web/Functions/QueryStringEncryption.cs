using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.Collections.Specialized;
using System.Security.Cryptography;
using System.IO;

namespace KMI.P3.Web.Functions
{

    /// <summary>
    /// Query String Helper
    /// </summary>
    public class QueryStringEncryption
    {

        //Crypate AdExpress
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

       

        /// <summary>
        ///  Crypt AdScope string
        /// </summary>
        /// <param name="s">String to encrypt</param>
        /// <returns></returns>
        public static string AdScopeCrypt(string s)
        {
            int key = 1234;
            int len = s.Length;
           Encoding encoding =  Encoding.GetEncoding("iso-8859-1");

           byte[] b = encoding.GetBytes(s); //System.Text.ASCIIEncoding.UTF32.GetBytes(s);
            byte[] buffer = new byte[len];

            for (int i = 0; i < len; i++)
            {
                buffer[i] = (byte)(ByteUnsignedValue(b[i]) ^ key);
            }
            return encoding.GetString(buffer); //ASCIIEncoding.UTF32.GetString(buffer);
        }


        private static uint ByteUnsignedValue(byte b)
        {           
            uint result = (uint)(b & 0x7FFFFFFF);          
                if ((b & 0x80000000) != 0)
                    return (result | 0x80000000);            
            return result;
        }


      
        public static string HexAsciiConvert(string HexValue)
        {
            string[] hexValuesSplit = HexValue.Split(';');
            string StrValue = "";
            foreach (String hex in hexValuesSplit)
            {
                if (!string.IsNullOrEmpty(hex))// && "&amp" != hex
                {
                    
                    StrValue += System.Convert.ToChar(System.Convert.ToUInt32(hex.Substring(3, 2), 16)).ToString();                   
                    
                }
            }
            return StrValue;
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
