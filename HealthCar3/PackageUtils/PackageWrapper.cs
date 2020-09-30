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
                Tag = tag,
                Data = data
            };
            return command;
        }
    }
}
