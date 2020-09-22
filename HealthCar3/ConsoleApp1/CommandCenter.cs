using ConsoleApp1.command.scene.terrain;
using Newtonsoft.Json;
using System;


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

            

            VpnCommand tunnel = new DunnyTunnel();
            VpnCommand terrainAdd = new TerrainAdd();
            tunnel.data.SetData(terrainAdd);

            Console.WriteLine(JsonConvert.SerializeObject(tunnel, serializerSettings));  
        }


    }
}
