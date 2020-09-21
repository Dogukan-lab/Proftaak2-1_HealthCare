using System;
using Newtonsoft.Json;

namespace ConsoleApp1
{
    class VpnCommand : IPayload
    {
        public string id;

        public VpnCommand(string id)
        {
            this.id = id;
        }
    }
}
