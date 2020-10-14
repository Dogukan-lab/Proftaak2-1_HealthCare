using System;
using System.Collections.Generic;
using System.Text;

namespace PackageUtils
{
    class CredentialVarificator
    {
        public static bool VerifyUserName(string name)
        {
            // TODO make an actual check
            if (name != "")
                return true;
            else
                return false;
        }
    }
}
