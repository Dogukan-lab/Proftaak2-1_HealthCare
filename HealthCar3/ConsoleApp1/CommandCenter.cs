using ConsoleApp1.command.scene.node;
using ConsoleApp1.data;
using ConsoleApp1.data.components;
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.Windows.Forms;

namespace ConsoleApp1
{
    class CommandCenter
    {
        private VpnConnector connector;
        private VpnCommand<ConnectData> tunnel;
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
            VpnCommand<DunnyData> dunny = new DunnyCommand();
            Encapsulate(dunny);
            Console.WriteLine(JsonConvert.SerializeObject(tunnel, serializerSettings));
        }


        private dynamic buildTransform(float x, float y, float z)
        {
            return new
            {
                position = new[] { x, y, z },
                scale = new[] { 1, 1, 1 }
            };
        }

        /**
         * TestFunction for generating an object.
         */
        private void GenerateObject()
        {
            NodeData data = new NodeData();
            

            VpnCommand<NodeData> addObject = new NodeAdd();
            addObject.data.SetName("B2-Object-Add-Test");
            addObject.data.SetComponents(new ComponentMashup(
                new TransformComponent(0, 0, 0, 1, 0, 0, 0),
                new ModelComponent("filelocation", true, false, "AnimationLocation"),
                new TerrainComponent(true),
                new PanelComponent(1, 1, 512, 512, 1, 1, 1, 1, true),
                new WaterComponent(20, 20, 0.1)));

            Encapsulate(addObject);


           

            dynamic packetData = new
            {
                name = "B2-Object-Add-Test",
                data = new
                {
                    transform = new
                    {
                        position = new[] { 0, 0, 0 },
                        scale = new[] { 1, 1, 1 }
                    },
                    transform2 = buildTransform(0, 0, 0)
                }
            };


            dynamic tunnelWrapper = new
            {
                id = "tunnel/send",
                data = new
                {
                    dest = "1238492348234jr89j",
                    data = packetData
                }
            };


            Console.WriteLine(JsonConvert.SerializeObject(tunnelWrapper, serializerSettings));
        }

        private void Encapsulate(IPayload payload)
        {
            this.tunnel.data.SetData(payload);
        }
    }
}
