﻿using System;
using System.IO;
using System.Security.Cryptography;

namespace Encryption.Shared
{
    public class Decryptor
    {
        private RSAParameters _rsaPrivateKey;
        private byte[] _aesKey;
        private byte[] _aesIv;

        /**
         * Decrypts data using RSA and AES.
         */
        public Decryptor()
        {
            
        }

        /*
         * setter property for RSA private key.
         */
        public RSAParameters RsaPrivateKey
        {
            set => _rsaPrivateKey = value;
        }

        /*
         * setter property for AES key.
         */
        public byte[] AesKey
        {
            set => _aesKey = value; 
        }

        /*
         * setter property for AES iv.
         */
        public byte[] AesIv
        {
            set => _aesIv = value; 
        }
        
        /**
         * Decrypts and RSA encrypted key.
         */
        public dynamic DecryptRSA(byte[] key)
        {
            try
            {
                byte[] decryptedData;
                //Create a new instance of RSACryptoServiceProvider.
                using (RSACryptoServiceProvider Rsa = new RSACryptoServiceProvider())
                {
                    //Import the RSA Key information. This needs
                    //to include the private key information.
                    Rsa.ImportParameters(_rsaPrivateKey);

                    //Decrypt the passed byte array and specify OAEP padding.  
                    //OAEP padding is only available on Microsoft Windows XP or
                    //later.  
                    decryptedData = Rsa.Decrypt(key, false);
                }
                return decryptedData;
            }
            //Catch and display a CryptographicException  
            //to the console.
            catch (CryptographicException e)
            {
                Console.WriteLine(e.ToString());
                return null;
            }
        }

        /**
         * Decrypts and AES encrypted packet.
         */
        public string DecryptAES(byte[] data, int index, int count)
        {
            // Check arguments.
            if (data == null || data.Length <= 0)
            {
                throw new ArgumentException("Data does not contain a message!");
            }
            
            byte[] chunk = new byte[count];
            for (int i = index; i < count; i++)
            {
                chunk[i] = data[i];
            }

            // Declare the string used to hold
            // the decrypted text.
            string decrypted = null;

            // Create an Aes object
            // with the specified key and IV.
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = _aesKey;
                aesAlg.IV = _aesIv;

                // Create a decryptor to perform the stream transform.
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for decryption.
                using (MemoryStream msDecrypt = new MemoryStream(chunk))
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