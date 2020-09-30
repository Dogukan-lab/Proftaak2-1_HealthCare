using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using System.Security.Cryptography.Xml;
using System.IO;
using System.Drawing;

namespace ConsoleApp1.encryption
{
    static class Encryptor
    {
        // key and IV statically hardcoded for conviencence.
        public static byte[] key = { 144, 39, 92, 101, 2, 110, 84, 28, 184, 39, 121, 251, 50, 171, 59, 117, 214, 161, 108, 111, 231, 202, 203, 99, 51, 40, 17, 158, 175, 175, 191, 85 };
        public static byte[] iv = { 218, 146, 160, 230, 45, 68, 214, 92, 176, 159, 170, 162, 174, 58, 161, 186 };


        public static byte[] Encrypt(string original)
        {
            // Check arguments.
            if (original == null || original.Length <= 0)
            {
                throw new ArgumentException("String does not contain a message!");
            }

            byte[] encrypted;

            // Create an Rijndael object
            // with the specified key and IV.
            using (Rijndael rijndael = Rijndael.Create())
            {
                rijndael.Key = key;
                rijndael.IV = iv;

                // Create an encryptor to perform the stream transform.
                ICryptoTransform encryptor = rijndael.CreateEncryptor(rijndael.Key, rijndael.IV);

                // Create the streams used for encryption.
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {

                            //Write all data to the stream.
                            swEncrypt.Write(original);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }
            // Return the encrypted bytes from the memory stream.
            return encrypted;
        }

        public static string Decrypt(byte[] encrypted)
        {
            // Check arguments.
            if (encrypted == null || encrypted.Length <= 0)
            {
                throw new ArgumentException("Data does not contain a message!");
            }

            // Declare the string used to hold
            // the decrypted text.
            string decrypted = null;

            // Create an Rijndael object
            // with the specified key and IV.
            using (Rijndael rijAlg = Rijndael.Create())
            {
                rijAlg.Key = key;
                rijAlg.IV = iv;

                // Create a decryptor to perform the stream transform.
                ICryptoTransform decryptor = rijAlg.CreateDecryptor(rijAlg.Key, rijAlg.IV);

                // Create the streams used for decryption.
                using (MemoryStream msDecrypt = new MemoryStream(encrypted))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {

                            // Read the decrypted bytes from the decrypting stream
                            // and place them in a string.
                            decrypted = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
            return decrypted;
        }
    }
}
