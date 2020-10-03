using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

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

        public static byte[] SerializeData(string tag, dynamic data)
        {
            return Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(WrapWithTag(tag, data)));
        }
    }
}
