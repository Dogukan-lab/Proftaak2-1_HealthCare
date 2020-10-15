using BikeApp.connections;
using Newtonsoft.Json;

namespace BikeApp
{
    internal static class Program
    {
        private static void Main()
        {
            Client.Initialize();
            // var vpnConnector = new VpnConnector(new JsonSerializerSettings());
        }
    }
}


