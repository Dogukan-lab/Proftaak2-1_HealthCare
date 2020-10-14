using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace PackageUtils
{
    class CredentialVarificator
    {
        /*
         * Checks if the received name only contains chars of a-z, A-Z and spaces, and make sure that there is atleast one char.
         */
        public static bool VerifyUsername(string name)
        {
            Regex validCharacters = new Regex(@"^[a-zA-Z\ ]+$");

            return validCharacters.IsMatch(name);
        }
    }
}
