
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace wDualAssistence.Helpers
{
    public static class Cryptography
    {

        public static RijndaelManaged GetRijndaelManaged(String secretKey)
        {
            var keyBytes = new byte[16];
            var secretKeyBytes = Encoding.UTF8.GetBytes(secretKey);
            Array.Copy(secretKeyBytes, keyBytes, Math.Min(keyBytes.Length, secretKeyBytes.Length));
            return new RijndaelManaged
            {
                Mode = CipherMode.CBC,
                Padding = PaddingMode.PKCS7,
                KeySize = 128,
                BlockSize = 128,
                Key = keyBytes,
                IV = keyBytes
            };
        }

        public static byte[] Encrypt(byte[] plainBytes, RijndaelManaged rijndaelManaged)
        {
            return rijndaelManaged.CreateEncryptor()
                .TransformFinalBlock(plainBytes, 0, plainBytes.Length);
        }

        public static byte[] Decrypt(byte[] encryptedData, RijndaelManaged rijndaelManaged)
        {
            return rijndaelManaged.CreateDecryptor()
                .TransformFinalBlock(encryptedData, 0, encryptedData.Length);
        }

        /// <summary>
        /// Encrypts plaintext using AES 128bit key and a Chain Block Cipher and returns a base64 encoded string
        /// </summary>
        /// <param name="plainText">Plain text to encrypt</param>
        /// <param name="key">Secret key</param>
        /// <returns>Base64 encoded string</returns>
        public static String Encrypt(String plainText, String key)
        {
            var plainBytes = Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(Encrypt(plainBytes, GetRijndaelManaged(key)));
        }

        /// <summary>
        /// Decrypts a base64 encoded string using the given key (AES 128bit key and a Chain Block Cipher)
        /// </summary>
        /// <param name="encryptedText">Base64 Encoded String</param>
        /// <param name="key">Secret Key</param>
        /// <returns>Decrypted String</returns>
        public static String Decrypt(String encryptedText, String key)
        {
            var encryptedBytes = Convert.FromBase64String(encryptedText);
            return Encoding.UTF8.GetString(Decrypt(encryptedBytes, GetRijndaelManaged(key)));
        }

        //--------------------------------MD5 para el empleado
        public static string EncriptarMD5(string texto)
        {
            MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
            byte[] hashInBytes = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(texto));

            string sResult = BitConverter.ToString(hashInBytes).Replace("-", string.Empty);

            return sResult.ToUpper();
        }

        public static string DesencriptarMD5(string textoEncriptado)
        {
            byte[] keyArray;
            //convierte el texto en una secuencia de bytes
            byte[] Array_a_Descifrar =
            Convert.FromBase64String(textoEncriptado);

            string ser = UTF8Encoding.UTF8.GetString(Array_a_Descifrar);


            //se llama a las clases que tienen los algoritmos
            //de encriptación se le aplica hashing
            //algoritmo MD5
            MD5CryptoServiceProvider hashmd5 =
            new MD5CryptoServiceProvider();

            keyArray = hashmd5.ComputeHash(
            UTF8Encoding.UTF8.GetBytes(textoEncriptado));

            string sfgfdf = UTF8Encoding.UTF8.GetString(keyArray);
            hashmd5.Clear();

            //se regresa en forma de cadena
            return UTF8Encoding.UTF8.GetString(Array_a_Descifrar);
        }

    }
}
