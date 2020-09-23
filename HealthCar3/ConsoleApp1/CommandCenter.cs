using ConsoleApp1.command.scene.node;
using ConsoleApp1.data;
using ConsoleApp1.data.components;
using Newtonsoft.Json;
using System;

namespace ConsoleApp1
{
    class CommandCenter
    {
        private VpnConnector connector;
        private VpnCommand tunnel;
        private JsonSerializerSettings serializerSettings;

        /**
         * Controller for managing construction and management of commands.
         */
        public CommandCenter()
        {
            serializerSettings = new JsonSerializerSettings();
            serializerSettings.NullValueHandling = NullValueHandling.Ignore;
            this.tunnel = new DunnyTunnel("dest");

            GenerateObject();
        }

        /**
         * Testfunction for testing tunnel command.
         */
        private void TestTunnel()
        {
            VpnCommand dunny = new DunnyCommand();
            Encapsulate(dunny);
            Console.WriteLine(JsonConvert.SerializeObject(tunnel, serializerSettings));
        }

        /**
         * TestFunction for generating an object.
         */
        private void GenerateObject()
        {
            NodeData data = new NodeData();
            data.SetName("B2-Object-Add-Test");
            
            data.SetComponents(new ComponentMashup(
                new TransformComponent(0, 0, 0, 1, 0, 0, 0),
                new ModelComponent("filelocation", true, false, "AnimationLocation"),
                new TerrainComponent(true), 
                new PanelComponent(1, 1, 512, 512, 1, 1, 1, 1, true), 
                new WaterComponent(20, 20, 0.1)));

            VpnCommand addObject = new NodeAdd(data);
            Encapsulate(addObject);
            Console.WriteLine(JsonConvert.SerializeObject(tunnel, serializerSettings));
        }

        private void Encapsulate(IPayload payload)
        {
            this.tunnel.data.SetData(payload);
        }
    }
}
