namespace Server
{
    public class ClientCredentials
    {
        public string id;
        public string password;
        public string username;

        /*
        * Puts all the data of the client into a dynamic and returns it.
        */
        public dynamic GetCredentials()
        {
            return new
            {
                username,
                password,
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