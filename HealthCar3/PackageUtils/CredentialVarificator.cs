using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace PackageUtils
{
    class CredentialVarificator
    {
        public static bool VerifyUsername(string name)
        {
            Regex validCharacters = new Regex(@"^[a-zA-Z\ ]+$");

            return validCharacters.IsMatch(name);
        }
    }
}
