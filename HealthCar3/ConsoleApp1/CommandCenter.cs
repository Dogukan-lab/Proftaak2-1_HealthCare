using ConsoleApp1.command.route;
using ConsoleApp1.command.scene;
using ConsoleApp1.data;
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
            connector = vpn;
        }

        public void ResetScene()
        {
            Console.WriteLine("Command: RESET SCENE");
            this.connector.SendPacket(Scene.Reset(), new Action<JObject>(data =>
            {
                //Console.WriteLine("Scene has been reset! Data: {0}", data.ToString());

            }));
        }

        public void CreateTerrain()
        {
            int[] heightMap = new int[65536];
            Random random = new Random();

            //Initializes the heightmap for the scene
            for (int i = 0; i < heightMap.Length; i++)
            {
                //Make the heightmap morer fancy TODO
                heightMap[i] = 0;
               
            }

            connector.SendPacket(Terrain.Add(new int[] { 256, 256 }, heightMap), new Action<JObject>(data =>
            {
                CreateTerrainTexture();
            }));

        }

        private void CreateTerrainTexture()
        {
            var uuid = "";
            this.connector.SendPacket(Node.AddTerrain("groundPlane", null, null, true), new Action<JObject>(data =>
            {
                uuid = data["data"]["data"]["uuid"].ToString();
                this.connector.SendPacket(Node.AddLayer(uuid, GetTextures("terrain/grass_green_d.jpg"), GetTextures("terrain/grass_normal.jpg"), 0, 10, 1), new Action<JObject>(data =>
                {
                    Console.WriteLine("Texture Data: {0}", data);
                }));
            }));
        }

        public void CreateRoute()
        {
            RouteData[] routeData = new RouteData[4];
            // Defining route
            routeData[0] = new RouteData(new int[] { 1, 0, 1}, new int[] { 1, 0, 0 });
            routeData[1] = new RouteData(new int[] { 11, 0, 1 }, new int[] { 0, 0, 1 });
            routeData[2] = new RouteData(new int[] { 11, 0, 11 }, new int[] { -1, 0, 0 });
            routeData[3] = new RouteData(new int[] { 1, 0, 11 }, new int[] { 0, 0, -1 });

            this.connector.SendPacket(Route.Add(routeData), new Action<JObject>(data =>
            {
                Console.WriteLine($"Response add: {data}");
                this.connector.SendPacket(Route.ShowRoute(true), new Action<JObject>(data =>
                {
                    Console.WriteLine($"Response show: {data}");
                }));
                string roadID = data["data"]["data"]["uuid"].ToString();
                AddRoad(data["data"]["data"]["uuid"].ToString());
                this.connector.SendPacket(Node.AddModel("Fiets", new TransformComponent(1, 0, 1, 1, 0, 0, 0), new ModelComponent(GetModelObjects("bike/bike.fbx"), true, false, "")), new Action<JObject>(data =>
                {
                    this.connector.SendPacket(Route.Follow(roadID, data["data"]["data"]["uuid"].ToString(),1, 0, "XZ", 1,false , new int[] { 0,0,0}, new int[] {0,0,0 } ), new Action<JObject>(data =>
                    { 
                    }));
                }));
               
            }));
        }

        private void AddRoad(string uuid)
        {
            this.connector.SendPacket(Road.AddRoad(uuid, GetTextures("tarmac_diffuse.png"), GetTextures("tarmac_normal.png"), GetTextures("tarmax_specular.png"), 0), new Action<JObject>(data =>
            {
                Console.WriteLine($"Response show: {data}");
            }));
        }

        public void CreateObject(string desiredModel)
        {
            this.connector.SendPacket(Node.AddModel("car", new TransformComponent(2, 2, 2, 1, 0, 0, 0), new ModelComponent(GetModelObjects(desiredModel), true, false, "")), new Action<JObject>(data =>
            {
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
            return $"data/NetworkEngine/textures/{textureName}";
        }
    }
}
