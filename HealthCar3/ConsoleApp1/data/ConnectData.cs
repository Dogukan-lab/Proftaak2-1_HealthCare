using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp1.data
{
    class ConnectData : VpnData
    {
        public string session, key, dest;

        public ConnectData()
        {

        }

        public string GetSession()
        {
            return this.session;
        }

        public string GetKey()
        {
            return this.key;
        }

        public string GetDestination()
        {
            return this.dest;
        }

        public void SetSession(string session)
        {
            this.session = session;
        }

        public void SetKey(string key)
        {
            this.key = key;
        }

        public void SetDestination(string destination)
        {
            this.dest = destination;
        }
    }
}
