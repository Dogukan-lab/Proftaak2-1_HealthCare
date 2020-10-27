using Newtonsoft.Json;
using System.Text;
using Encryption.Shared;
using System.Linq;
using System;

namespace PackageUtils
{
    internal static class PackageWrapper
    {
        private static dynamic WrapWithTag(string tag, dynamic data)
        {
            dynamic command = new
            {
                tag, data
            };
            return command;
        }

        /**
         * Wraps the data without encryption.
         */
        public static byte[] SerializeData(string tag, dynamic data)
        {
            byte[] message = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(WrapWithTag(tag, data)));
            byte[] lengthPrefix = BitConverter.GetBytes(message.Length);
            message = lengthPrefix.Concat(message).ToArray();
            return message;
        }

        /**
         * Wraps the data with encryption.
         */
        public static byte[] SerializeData(string tag, dynamic data, Encryptor encryptor)
        {
            byte[] message = encryptor.EncryptAes(JsonConvert.SerializeObject(WrapWithTag(tag, data)));
            byte[] lengthPrefix = BitConverter.GetBytes(message.Length);
            message = lengthPrefix.Concat(message).ToArray();
            return message;
        }
    }
}
