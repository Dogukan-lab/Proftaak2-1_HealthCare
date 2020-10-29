using System;
using System.IO;
using System.Security.Cryptography;

namespace Encryption.Shared
{
    public class Decryptor
    {
        private RSAParameters rsaPrivateKey;
        private byte[] aesKey;
        private byte[] aesIv;

        /*
         * Setter property for RSA private key.
         */
        public RSAParameters RsaPrivateKey
        {
            set => rsaPrivateKey = value;
        }

        /*
         * Setter property for AES key.
         */
        public byte[] AesKey
        {
            set => aesKey = value; 
        }

        /*
         * Setter property for AES iv.
         */
        public byte[] AesIv
        {
            set => aesIv = value; 
        }
        
        /**
         * Decrypts and RSA encrypted key.
         */
        public dynamic DecryptRsa(byte[] key)
        {
            try
            {
                //Create a new instance of RSACryptoServiceProvider.
                using var rsa = new RSACryptoServiceProvider();
                //Import the RSA Key information. This needs
                //to include the private key information.
                rsa.ImportParameters(rsaPrivateKey);

                //Decrypt the passed byte array and specify OAEP padding.  
                //OAEP padding is only available on Microsoft Windows XP or
                //later.  
                var decryptedData = rsa.Decrypt(key, false);
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
        public string DecryptAes(byte[] data, int index, int count)
        {
            // Check arguments.
            if (data == null || data.Length <= 0)
            {
                throw new ArgumentException("Data does not contain a message!");
            }
            
            var chunk = new byte[count];
            for (var i = index; i < count; i++)
            {
                chunk[i] = data[i];
            }

            // Declare the string used to hold
            // the decrypted text.

            // Create an Aes object
            // with the specified key and IV.
            using var aesAlg = Aes.Create();
            if (aesAlg == null) return null;
            aesAlg.Key = aesKey;
            aesAlg.IV = aesIv;

            // Create a decryptor to perform the stream transform.
            var decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

            // Create the streams used for decryption.
            using var msDecrypt = new MemoryStream(chunk);
            using var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
            using var srDecrypt = new StreamReader(csDecrypt);
            // Read the decrypted bytes from the decrypting stream
            // and place them in a string.
            var decrypted = srDecrypt.ReadToEnd();

            return decrypted;
        }
        
    }
}