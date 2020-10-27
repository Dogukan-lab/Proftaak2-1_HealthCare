using BikeApp.connections;
using Newtonsoft.Json;
using System;
using System.Threading;

namespace BikeApp
{
    internal static class Program
    {
       [STAThread]
        private static void Main()
        {
            Client.Initialize();            
            // var vpnConnector = new VpnConnector(new JsonSerializerSettings());
        }
    }
}


