using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Lib.Strings;

namespace Lib.Security
{
    public static class SecurityFunctions
    {
        public static string TripleDESDecryptFramework(string data, string key)
        {
            return TripleDESDecryptFramework(StringsFunctions.StringToUtf8Bytes(data), StringsFunctions.StringToUtf8Bytes(key));
        }

        public static TripleDES InitTripleDES(object p)
        {
            throw new NotImplementedException();
        }

        public static string TripleDESDecryptFramework(string data, byte[] key)
        {
            return TripleDESDecryptFramework(StringsFunctions.StringToUtf8Bytes(data), key);
        }

        public static string TripleDESDecryptFramework(byte[] plainData, string key)
        {
            return TripleDESDecryptFramework(plainData, StringsFunctions.StringToUtf8Bytes(key));
        }

        public static string TripleDESDecryptFramework(byte[] plainData, byte[] key)
        {
            TripleDES tripleDES = InitTripleDES(key);

            byte[] result;

            using (ICryptoTransform decryptor1 = tripleDES.CreateDecryptor())
            {
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor1, CryptoStreamMode.Write))
                    {
                        cryptoStream.Write(plainData, 0, plainData.Length);
                    }

                    result = memoryStream.ToArray();
                }
            }

            return Encoding.UTF8.GetString(result);
        }

        //

        public static byte[] TripleDESEncryptFramework(string plainText, string key)
        {
            return TripleDESEncryptFramework(StringsFunctions.StringToUtf8Bytes(plainText), StringsFunctions.StringToUtf8Bytes(key));
        }

        public static byte[] TripleDESEncryptFramework(string plainText, byte[] key)
        {
            return TripleDESEncryptFramework(StringsFunctions.StringToUtf8Bytes(plainText), key);
        }

        public static byte[] TripleDESEncryptFramework(byte[] plainText, string key)
        {
            return TripleDESEncryptFramework(plainText, StringsFunctions.StringToUtf8Bytes(key));
        }

        public static byte[] TripleDESEncryptFramework(byte[] plainText, byte[] key)
        {
            TripleDES tripleDES = InitTripleDES(key);

            byte[] result;

            using (ICryptoTransform encryptor = tripleDES.CreateEncryptor())
            {
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                    {
                        cryptoStream.Write(plainText, 0, plainText.Length);
                    }

                    result = memoryStream.ToArray();
                }
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns>
        ///     Initialized class
        /// </returns>
        public static TripleDES InitTripleDES(byte[] key)
        {
            TripleDES result = TripleDES.Create();
            result.Mode = CipherMode.ECB;
            result.Padding = PaddingMode.PKCS7;
            result.Key = key;

            return result;
        }
    }
}
