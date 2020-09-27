using ConsoleApp1.command.scene;
using ConsoleApp1.data.components;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Reflection;

namespace ConsoleApp1
{
    class CommandCenter
    {
        private VpnConnector connector;
        private JsonSerializerSettings serializerSettings;
        private string runningPath;

        /**
         * Controller for managing construction and management of commands.
         * Om bomen aan het landschap toe te voegen maak het terrein naam de parent.
         * TODO callbacks are being returned twice
         * so the callbacks need to be fixed
         */
        public CommandCenter(VpnConnector vpn)
        {
            string uuid = "";
            connector = vpn;
        }

        public void ResetScene()
        {
            Console.WriteLine("Command: RESET SCENE");
            this.connector.SendPacket(Scene.Reset(), new Action<JObject>(data =>
            {
                Console.WriteLine("Scene has been reset! Data: {0}", data.ToString());

            }));
            //CreateTerrain();

        }

        private void CreateTerrain()
        {
            int[] heightMap = new int[65536];
            Random random = new Random();

            //Initializes the heightmap for the scene
            for (int i = 0; i < heightMap.Length; i++)
            {
                heightMap[i] = random.Next(0, 5);
            }

            connector.SendPacket(Terrain.Add(new int[] { 256, 256 }, heightMap), new Action<JObject>(data =>
            {
                Console.WriteLine("Terrain skeleton added!");
            }));
            CreateTerrainTexture();

        }

        private void CreateTerrainTexture()
        {
            string uuid = "";
            this.connector.SendPacket(Node.AddTerrain("groundPlane", null, null, true), new Action<JObject>(data =>
            {
                Console.WriteLine("Data: {0}", data.ToString());
                uuid = data["data"]["data"]["uuid"].ToString();

            }));

            this.connector.SendPacket(Node.AddLayer(uuid, GetTextures("grass_diffuse.png"), GetTextures("grass_normal.png"), 0, 10, 1), new Action<JObject>(data =>
            {
                Console.WriteLine("Texture has been added!");
            }));

        }

        public void CreateObject(string desiredModel)
        {
            Console.WriteLine("Command: ADD CAR OBJECT");

            this.connector.SendPacket(Node.AddModel("car", new TransformComponent(2, 2, 2, 1, 0, 0, 0), new ModelComponent(GetModelObjects(desiredModel), true, false, "")), new Action<JObject>(data =>
            {
                Console.WriteLine("Node added {0}", data);
            }));
        }

        /**
         * Returns the root path with the given file name
         */
        private string GetResourcePath(string fileName)
        {
            return Path.Join(runningPath, fileName);
        }

        private string GetModelObjects(string objectname)
        {
            return $"data/NetworkEngine/models/{objectname}";
        }

        private string GetTextures(string textureName)
        {
            return $"data/NetworkEngine/texture/{textureName}";
        }
    }
}
