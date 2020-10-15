using Newtonsoft.Json;
using System.Text;
using Encryption.Shared;

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
