using System;
using System.Collections.Generic;
using System.Text;

namespace Server
{
    class ClientData
    {
        public string userName;
        public string passWord;

        /*
        * Puts all the data of the client into a dynamic and returns it.
        */
        public dynamic GetData()
        {
            return new
            {
                userName = userName,
                passWord = passWord,              
            };
        }
    }
}
