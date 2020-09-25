using ConsoleApp1.command.scene;
using ConsoleApp1.command.scene.node;
using ConsoleApp1.command.scene.terrain;
using ConsoleApp1.data;
using ConsoleApp1.data.components;
using Newtonsoft.Json;
using System;

namespace ConsoleApp1
{
    class CommandCenter
    {
        private VpnConnector connector;
        private JsonSerializerSettings serializerSettings;

        /**
         * Controller for managing construction and management of commands.
         */
        public CommandCenter(String dest, VpnConnector vpn)
        {
            serializerSettings = new JsonSerializerSettings();
            serializerSettings.NullValueHandling = NullValueHandling.Ignore;
            connector = vpn;
            
            GenerateObject();

            VpnCommand tunnel = new DunnyTunnel(dest);
            CreateTerrain(tunnel);

            connector.Send(tunnel);

            
        }

        /**
         * Testfunction for testing tunnel command.
         */
        private void TestTunnel()
        {
            VpnCommand dunny = new DunnyCommand();
            VpnCommand tunnel = new DunnyTunnel("dest");

            tunnel.data.SetData(dunny);

            Console.WriteLine(JsonConvert.SerializeObject(tunnel, serializerSettings));
        }

        /**
         * TestFunction for generating an object.
         */
        private void GenerateObject()
        {
            VpnCommand tunnel = new DunnyTunnel("dest");

            NodeData data = new NodeData();
            data.SetName("B2-Object-Add-Test");
            
            data.SetComponents(new ComponentMashup(
                new TransformComponent(0, 0, 0, 1, 0, 0, 0),
                new ModelComponent("filelocation", true, false, "AnimationLocation"),
                new TerrainComponent(true), 
                new PanelComponent(1, 1, 512, 512, 1, 1, 1, 1, true), 
                new WaterComponent(20, 20, 0.1)));

            VpnCommand addObject = new NodeAdd(data);
            tunnel.data.SetData(addObject);

            Console.WriteLine(JsonConvert.SerializeObject(tunnel, serializerSettings));

        }

        private void CreateTerrain(VpnCommand tunnel)
        {
            TerrainData data = new TerrainData();

            data.SetSize(256, 256);
            data.SetHeight(0);
            VpnCommand terraiAdd = new TerrainAdd(data);

            tunnel.data.SetData(terraiAdd);
            
        }

        public void createBoundries()
        {

        }


    }
}
