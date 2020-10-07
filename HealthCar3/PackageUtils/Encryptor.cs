﻿using System;
using System.ComponentModel;
using System.IO;
using System.Security.Cryptography;

namespace Testroom.Encryption.Shared
{
    public class Encryptor
    {
        private byte[] _aesKey;
        private byte[] _aesIv;
        
        /**
         * Encrypts data using RSA and AES.
         */
        public Encryptor()
        {
            
        }

        /*
         * setter property for EAS key.
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
         * Generates a new RSA keySet and returns both the private and public keys.
         */
        public (RSAParameters privKey, RSAParameters pubKey) GenerateRsaKey()
        {
            RSAParameters privateKey;
            RSAParameters pubKey;

            //Create a new instance of RSACryptoServiceProvider to generate
            //public and private key data.
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
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
            using (Rijndael rijndael = Rijndael.Create())
            {
                _aesKey = rijndael.Key;
                _aesIv = rijndael.IV;
            }
            
            return (_aesKey, _aesIv);
        }

        /**
         * Encrypts and AES key using the RSA public key.
         */
        public (byte[] key, byte[] iv) EncryptRsa(byte[] key, byte[] iv, RSAParameters pubkey)
        {
            try
            {
                byte[] encryptedKey;
                byte[] encryptedIv;
                //Create a new instance of RSACryptoServiceProvider.
                using (RSACryptoServiceProvider RSA = new RSACryptoServiceProvider())
                {

                    //Import the RSA Key information. This only needs
                    //to include the public key information.
                    RSA.ImportParameters(pubkey);

                    //Encrypt the passed byte array and specify OAEP padding.  
                    //OAEP padding is only available on Microsoft Windows XP or
                    //later.  
                    encryptedKey = RSA.Encrypt(key, false);
                    encryptedIv = RSA.Encrypt(iv, false);
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
                throw new ArgumentNullException("data");
            if (_aesKey == null || _aesKey.Length <= 0)
                throw new ArgumentNullException("Key");
            if (_aesIv == null || _aesIv.Length <= 0)
                throw new ArgumentNullException("IV");
            byte[] encrypted;
            // Create an Rijndael object
            // with the specified key and IV.
            using (Rijndael rijAlg = Rijndael.Create())
            {
                rijAlg.Key = _aesKey;
                rijAlg.IV = _aesIv;

                // Create an encryptor to perform the stream transform.
                ICryptoTransform encryptor = rijAlg.CreateEncryptor(rijAlg.Key, rijAlg.IV);

                // Create the streams used for encryption.
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {

                            //Write all data to the stream.
                            swEncrypt.Write(data);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }

            // Return the encrypted bytes from the memory stream.
            return encrypted;
        }
        
    }
}