using System;
using System.IO;
using System.Security.Cryptography;

namespace Encryption.Shared
{
    public class Encryptor
    {
        private byte[] aesKey;
        private byte[] aesIv;

        /*
         * setter property for EAS key.
         */
        public byte[] AesKey
        {
            set => aesKey = value;
        }

        /*
         * setter property for AES iv.
         */
        public byte[] AesIv
        {
            set => aesIv = value;
        }

        /**
         * Generates a new RSA keySet and returns both the private and public keys.
         */
        public (RSAParameters privKey, RSAParameters pubKey) GenerateRsaKey()
        {
            RSAParameters privateKey;
            RSAParameters pubKey;

            //Create a new instance of RSACryptoServiceProvider to generate
            //public and private key data.
            using (var rsa = new RSACryptoServiceProvider())
            {
                privateKey = rsa.ExportParameters(true); //true marks a private key.
                pubKey = rsa.ExportParameters(false); //false marks a public key.
            }

            return (privateKey, pubKey);
        }

        /**
         * Generates a new Aes Key and Iv pair.
         */
        public (byte[] key, byte[] iv) GenerateAesKey()
        {
            // Create a new instance of the Rijndael
            // class.  This generates a new key and initialization
            // vector (IV).
            using (var rijndael = Rijndael.Create())
            {
                aesKey = rijndael.Key;
                aesIv = rijndael.IV;
            }
            
            return (aesKey, aesIv);
        }

        /**
         * Encrypts and AES key using the RSA public key.
         */
        public (byte[] key, byte[] iv) EncryptRsa(byte[] key, byte[] iv, RSAParameters pubKey)
        {
            try
            {
                byte[] encryptedKey;
                byte[] encryptedIv;
                //Create a new instance of RSACryptoServiceProvider.
                using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
                {

                    //Import the RSA Key information. This only needs
                    //to include the public key information.
                    rsa.ImportParameters(pubKey);

                    //Encrypt the passed byte array and specify OAEP padding.  
                    //OAEP padding is only available on Microsoft Windows XP or
                    //later.  
                    encryptedKey = rsa.Encrypt(key, false);
                    encryptedIv = rsa.Encrypt(iv, false);
                }
                return (encryptedKey, encryptedIv);
            }
            //Catch and display a CryptographicException  
            //to the console.
            catch (CryptographicException e)
            {
                Console.WriteLine(e.Message);
                return (null, null);
            }
        }

        /**
         * Encrypts a packet using the AES key.
         */
        public byte[] EncryptAes(string data)
        {
            // Check arguments.
            if (data == null || data.Length <= 0)
                throw new ArgumentNullException(nameof(data));
            if (aesKey == null || aesKey.Length <= 0)
                throw new ArgumentNullException(nameof(data));
            if (aesIv == null || aesIv.Length <= 0)
                throw new ArgumentNullException(nameof(data));
            // Create an Aes object
            // with the specified key and IV.
            using var aesAlg = Aes.Create();
            if (aesAlg == null) return null;
            aesAlg.Key = aesKey;
            aesAlg.IV = aesIv;

            // Create an encryptor to perform the stream transform.
            var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

            // Create the streams used for encryption.
            using var msEncrypt = new MemoryStream();
            using var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write);
            using (var swEncrypt = new StreamWriter(csEncrypt))
            {
                //Write all data to the stream.
                swEncrypt.Write(data);
            }

            var encrypted = msEncrypt.ToArray();

            // Return the encrypted bytes from the memory stream.
            return encrypted;
        }
        
    }
}