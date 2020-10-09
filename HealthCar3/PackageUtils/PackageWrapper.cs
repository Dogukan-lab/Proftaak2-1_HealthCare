using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using Encryption.Shared;

namespace PackageUtils
{
    class PackageWrapper
    {
        public static dynamic WrapWithTag(string tag, dynamic data)
        {
            dynamic command = new
            {
                tag = tag,
                data = data
            };
            return command;
        }

        /**
         * Wraps the data without encryption.
         */
        public static byte[] SerializeData(string tag, dynamic data)
        {
            return Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(WrapWithTag(tag, data)));
        }

        /**
         * Wraps the data with encryption.
         */
        public static byte[] SerializeData(string tag, dynamic data, Encryptor encryptor)
        {
            return encryptor.EncryptAes(JsonConvert.SerializeObject(WrapWithTag(tag, data)));
        }
    }
}
