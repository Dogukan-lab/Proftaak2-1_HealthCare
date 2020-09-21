using System;
using Newtonsoft.Json;

namespace ConsoleApp1
{
    class VpnCommand : IPayload
    {
        public string id;
        public VpnData data;

        public VpnCommand(string id)
        {
            this.id = id;
        }

        public VpnCommand(string id, VpnData data)
        {
            this.id = id;
            this.data = data;
        }
    }
}
