using ConsoleApp1.command.scene.terrain;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp1
{
    class CommandCenter
    {
        private VpnConnector connector;
        public CommandCenter(string dest)
        {
            JsonSerializerSettings serializerSettings = new JsonSerializerSettings();
            serializerSettings.NullValueHandling = NullValueHandling.Ignore;
            //connector = new VpnConnector(serializerSettings);
            

            VpnCommand dunny = new TerrainAdd();
            VpnCommand tunnel = new DunnyTunnel(dest);

            tunnel.data.SetData(dunny);

            Console.WriteLine(JsonConvert.SerializeObject(tunnel, serializerSettings));
            //connector.Send(tunnel);

            
        }


    }
}
