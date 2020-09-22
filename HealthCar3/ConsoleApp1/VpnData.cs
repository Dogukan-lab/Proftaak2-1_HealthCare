using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Transactions;
using Newtonsoft.Json.Linq;
using System.Diagnostics.Eventing.Reader;

namespace ConsoleApp1
{
    /**
     * This class is used to store the parcable data.
     */
    class VpnData : IPayload
    {
        public string id;
        public string session, key, dest;
        public IPayload data;
        public int[] size;
        public int[] height;

        public VpnData()
        {

        }

        public VpnData(string id)
        {
            this.id = id;
        }

        /**
         * Simple getters and setters for the attributes
         */
        public IPayload GetData()
        {
            return this.data;
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

        public int[] GetSize()
        {
            return this.size;
        }

        public int[] GetHeight()
        {
            return this.height;
        }

        public void SetHeight(int val)
        {
            this.height = new int[] { val };
        }

        public void SetSize(int width, int height)
        {
            this.size = new int[] { width, height };
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

        public void SetData(IPayload data)
        {
            this.data = data;
        }
    }
}
