using ConsoleApp1.command.scene.node;
using ConsoleApp1.data;
using ConsoleApp1.data.components;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp1
{
    class CommandCenter
    {
        private VpnConnector connector;
        private JsonSerializerSettings serializerSettings;

        public CommandCenter()
        {
            serializerSettings = new JsonSerializerSettings();
            serializerSettings.NullValueHandling = NullValueHandling.Ignore;

            GenerateObject();
        }

        private void TestTunnel()
        {
            VpnCommand dunny = new DunnyCommand();
            VpnCommand tunnel = new DunnyTunnel();

            tunnel.data.SetData(dunny);

            Console.WriteLine(JsonConvert.SerializeObject(tunnel, serializerSettings));
        }

        private void GenerateObject()
        {
            NodeData data = new NodeData();
            data.SetName("B2-Object-Add-Test");

            
            
            
            ;
            
            data.SetComponents(new ComponentMashup(
                new TransformComponent(0, 0, 0, 1, 0, 0, 0),
                new ModelComponent("filelocation", true, false, "AnimationLocation"),
                new TerrainComponent(true), 
                new PanelComponent(1, 1, 512, 512, 1, 1, 1, 1, true), 
                new WaterComponent(20, 20, 0.1)));

            VpnCommand addObject = new NodeAdd(data);

            Console.WriteLine(JsonConvert.SerializeObject(addObject, serializerSettings));
        }


    }
}
