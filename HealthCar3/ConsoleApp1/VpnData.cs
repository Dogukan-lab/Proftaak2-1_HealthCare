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
        private readonly string id;
        private IPayload data;

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
        
        public void SetData(IPayload data)
        {
            this.data = data;
        }
    }
}
