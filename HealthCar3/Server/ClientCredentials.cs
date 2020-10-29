using System;
using System.Collections.Generic;
using System.Text;

namespace Server
{
    public class ClientCredentials
    {
        public string username;
        public string password;
        public string id;

        /*
        * Puts all the data of the client into a dynamic and returns it.
        */
        public dynamic GetCredentials()
        {
            return new
            {
                username = username,
                password = password,
                clientId = id
            };
        }

        
        public void SetCredentials(string clientUsername, string clientPassword, string clientId)
        {
            username = clientUsername;
            password = clientPassword;
            id = clientId;
        }
    }
}
