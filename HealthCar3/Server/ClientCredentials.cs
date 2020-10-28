using System;
using System.Collections.Generic;
using System.Text;

namespace Server
{
    public class ClientCredentials
    {
        public string _username;
        public string _password;
        public string _id;

        /*
        * Puts all the data of the client into a dynamic and returns it.
        */
        public dynamic GetCredentials()
        {
            return new
            {
                username = _username,
                password = _password,
                clientId = _id
            };
        }

        
        public void SetCredentials(string clientUsername, string clientPassword, string id)
        {
            _username = clientUsername;
            _password = clientPassword;
            _id = id;
        }
    }
}
