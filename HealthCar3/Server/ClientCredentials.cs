using System;
using System.Collections.Generic;
using System.Text;

namespace Server
{
    class ClientCredentials
    {
        public string username;
        public string password;

        /*
        * Puts all the data of the client into a dynamic and returns it.
        */
        public dynamic GetCredentials()
        {
            return new
            {
                userName = username,
                passWord = password,
            };
        }

        
        public void SetCredentials(string clientUsername, string clientPassword)
        {
            username = clientUsername;
            password = clientPassword;
        }
    }
}
