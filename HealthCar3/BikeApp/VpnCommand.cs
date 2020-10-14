using System;
using BikeApp.data;
using Newtonsoft.Json;

namespace BikeApp
{
    /**
     * Base command structure for all commands to be sent to the server.
     */
    class VpnCommand<DataType> : IPayload
        where DataType : VpnData, new()
    {
        public string id;
        public DataType data;

        /**
         * Base constructor for VpnCommands only requiring an id and no data.
         */
        public VpnCommand(string id)
        {
            this.id = id;
            this.data = new DataType();
        }

        /**
         * Alternative constructor for commands requiring both an id and data.
         */
        public VpnCommand(string id, DataType data)
        {
            this.id = id;
            this.data = data;
        }

        public DataType GetData()
        {
            return data;
        }
    }
}
