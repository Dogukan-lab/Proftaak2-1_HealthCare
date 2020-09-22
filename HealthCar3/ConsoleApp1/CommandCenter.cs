using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp1
{
    class CommandCenter
    {
        private VpnConnector connector;



        public CommandCenter()
        {
            JsonSerializerSettings serializerSettings = new JsonSerializerSettings();
            serializerSettings.NullValueHandling = NullValueHandling.Ignore;
            serializerSettings.DefaultValueHandling = DefaultValueHandling.Ignore;

            VpnCommand dunny = new DunnyCommand("scene/");
            VpnCommand dunny = new DunnyCommandAdd("scene/");

            VpnCommand tunnel = new DunnyTunnel();

            tunnel.data.SetData(dunny);

            Console.WriteLine(JsonConvert.SerializeObject(tunnel, serializerSettings));  
        }


    }
}
