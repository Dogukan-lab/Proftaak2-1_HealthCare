using System.Text.RegularExpressions;

namespace PackageUtils
{
    internal static class CredentialVerificator
    {
        public static bool VerifyUsername(string name)
        {
            var validCharacters = new Regex(@"^[a-zA-Z\ ]+$");
            return validCharacters.IsMatch(name);
        }
    }
}
