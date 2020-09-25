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
         */
        public CommandCenter(String dest, VpnConnector vpn)
        {
            string uuid = "";

            // Get the "root" path of our resources 
            runningPath = AppDomain.CurrentDomain.BaseDirectory;

            //serializerSettings = new JsonSerializerSettings();
            //serializerSettings.NullValueHandling = NullValueHandling.Ignore;
            connector = vpn;

            Action<JObject> addObject = new Action<JObject>(data =>
           {
               Console.WriteLine("Added node: {0}", data);
           });

            Action<JObject> cb2 = new Action<JObject>(data => {
                Console.WriteLine("Added Layer");
            });

            Action<JObject> cb1 = new Action<JObject>(data => {
                Console.WriteLine("Added Node");
                Console.WriteLine(data);
                connector.SendPacket(Node.AddLayer(uuid, @"data/NetwerkEngine/textures/grass_diffuse.png", @"data/NetwerkEngine/textures/grass_normal.png", 0, 10, 1), cb2);
                //connector.SendPacket(Node.AddLayer(uuid, GetResourcePath(@"resources\NetworkEngine\textures\terrain\grass_diffuse.png"), GetResourcePath(@"resources\NetworkEngine\textures\terrain\grass_normal.png"), 0, 10, 1), cb2);
            });

            int count = 0;
            Action<JObject> cb = new Action<JObject>(data => {
                Console.WriteLine("Added Terrain");
                connector.SendPacket(Node.AddTerrain("ground", null, null, true), cb1);
            });

            Random random = new Random();
            int[] heightMap = new int[65536];

            for (int i = 0; i < heightMap.Length; i++)
            {
                heightMap[i] = random.Next(0, 10000);
            }

            connector.SendPacket(Terrain.Add(new int[] { 256, 256 }, heightMap), cb);
            //connector.SendPacket(Node.AddModel("car", new TransformComponent(2, 2, 2, 0.01, 0, 0, 0), new ModelComponent(@"data/NetworkEngine/models/cars/cartoon/Pony_cartoon.obj", true, false, "")), addObject);
        }

        /**
         * Returns the root path with the given file name
         */
        private string GetResourcePath(string fileName)
        {
            return Path.Join(runningPath, fileName);
        }
    }
}
