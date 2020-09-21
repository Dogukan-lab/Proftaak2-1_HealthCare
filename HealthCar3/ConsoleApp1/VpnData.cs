﻿using System;
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
    class VpnData
    {
        public string id;
        public VpnData data;
        public string session, key, dest;

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
        public VpnData GetData()
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

        public void SetData(VpnData data)
        {
            this.data = data;
        }
    }
}
