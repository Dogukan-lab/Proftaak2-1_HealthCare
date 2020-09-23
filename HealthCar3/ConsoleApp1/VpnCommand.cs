using System;
using Newtonsoft.Json;

namespace ConsoleApp1
{
    /**
     * Base command structure for all commands to be sent to the server.
     */
    class VpnCommand : IPayload
    {
        public string id;
        public VpnData data;

        /**
         * Base constructor for VpnCommands only requiring an id and no data.
         */
        public VpnCommand(string id)
        {
            this.id = id;
        }

        /**
         * Alternative constructor for commands requiring both an id and data.
         */
        public VpnCommand(string id, VpnData data)
        {
            this.id = id;
            this.data = data;
        }
    }
}
