using ConsoleApp1.command.scene;
using ConsoleApp1.data;
using ConsoleApp1.data.components;
using Newtonsoft.Json.Linq;
using System;

namespace ConsoleApp1
{
    class CommandCenter
    {
        private VpnConnector connector;

        private string roadTexture,
            roadNormal,
            roadSpecular,
            terrainTexture,
            terrainNormal;

        private int routeSize;

        /**
         * Controller for managing construction and management of commands.
         * Om bomen aan het landschap toe te voegen maak het terrein naam de parent.
         */
        public CommandCenter(VpnConnector vpn)
        {
            connector = vpn;
            this.roadTexture = "";
            this.roadNormal = "";
            this.roadSpecular = "";
            this.terrainTexture = "snow_grass3_d.jpg";
            this.terrainNormal = "snow_grass3_n.jpg";
            this.routeSize = 10;
        }

        public void PresetOne()
        {
            ResetScene();
            CreateTerrain(this.terrainTexture, this.terrainNormal);

            RouteData[] routeData = new RouteData[7];
            // Defining route
            routeData[0] = new RouteData(new int[] {0, 0, 0}, new int[] {1, 0, 0});
            routeData[1] = new RouteData(new int[] {20, 0, 0}, new int[] {0, 0, 1});
            routeData[2] = new RouteData(new int[] {20, 0, 50}, new int[] {-1, 0, 0});
            routeData[3] = new RouteData(new int[] {-10, 0, 30}, new int[] {-1, 0, 0});
            routeData[4] = new RouteData(new int[] {-30, 0, 10}, new int[] {0, 0, 0});
            routeData[5] = new RouteData(new int[] {-20, 0, 0}, new int[] {0, 0, 0});
            routeData[6] = new RouteData(new int[] {-10, 0, -10}, new int[] {0, 0, -1});

            CreateRoute(routeData);
        }

        #region Scene Code
        public void SetTime(SkyBoxTime time)
        {
            switch (time)
            {
                case SkyBoxTime.MORNING:
                    this.connector.SendPacket(Skybox.SetTime(5), new Action<JObject>(data =>
                    {
                        this.connector.SendPacket(Skybox.Update("static",
                                GetSkyBox("bluecloud_rt.jpg"), GetSkyBox("bluecloud_lf.jpg"),
                                GetSkyBox("bluecloud_up.jpg"),
                                GetSkyBox("bluecloud_dn.jpg"), GetSkyBox("bluecloud_bk.jpg"),
                                GetSkyBox("bluecloud_ft.jpg")),
                            new Action<JObject>(data => { Console.WriteLine("It's daytime!"); }));
                    }));
                    break;
                case SkyBoxTime.AFTERNOON:
                    this.connector.SendPacket(Skybox.SetTime(12), new Action<JObject>(data =>
                    {
                        this.connector.SendPacket(Skybox.Update("static",
                                GetSkyBox("graycloud_rt.jpg"), GetSkyBox("graycloud_lf.jpg"),
                                GetSkyBox("graycloud_up.jpg"),
                                GetSkyBox("graycloud_dn.jpg"), GetSkyBox("graycloud_bk.jpg"),
                                GetSkyBox("graycloud_ft.jpg")),
                            new Action<JObject>(data => { Console.WriteLine("It's the afternoon!"); }));
                    }));
                    break;
                case SkyBoxTime.EVENING:
                    this.connector.SendPacket(Skybox.SetTime(18), new Action<JObject>(data =>
                    {
                        this.connector.SendPacket(Skybox.Update("static",
                                GetSkyBox("yellowcloud_rt.jpg"), GetSkyBox("yellowcloud_lf.jpg"),
                                GetSkyBox("yellowcloud_up.jpg"),
                                GetSkyBox("yellowcloud_dn.jpg"), GetSkyBox("yellowcloud_bk.jpg"),
                                GetSkyBox("yellowcloud_ft.jpg")),
                            new Action<JObject>(data => { Console.WriteLine("It's in the evening!"); }));
                    }));
                    break;
                case SkyBoxTime.NIGHT:
                    this.connector.SendPacket(Skybox.SetTime(22), new Action<JObject>(data =>
                    {
                        this.connector.SendPacket(Skybox.Update("static",
                                GetSkyBox("graycloud_rt.jpg"), GetSkyBox("graycloud_lf.jpg"),
                                GetSkyBox("graycloud_up.jpg"),
                                GetSkyBox("graycloud_dn.jpg"), GetSkyBox("graycloud_bk.jpg"),
                                GetSkyBox("graycloud_ft.jpg")),
                            new Action<JObject>(data => { Console.WriteLine("It's nightTime!"); }));
                    }));
                    break;
            }
        }



        public void GetScene()
        {
            this.connector.SendPacket(Scene.Get(),
                new Action<JObject>(data => { Console.WriteLine("Scene data: {0}", data); }));
        }

        /*
         * This method deletes the original groundplane and resets the entire scene. 
         */
        public void ResetScene()
        {
            this.connector.SendPacket(Scene.Reset(), new Action<JObject>(data =>
            {
                Console.WriteLine("Scene has been reset!");
                this.connector.SendPacket(Node.Find("GroundPlane"),
                    new Action<JObject>(data =>
                    {
                        Console.WriteLine("Scene data: {0}", data);
                        this.connector.SendPacket(Node.Delete(data["data"]["data"][0]["uuid"].ToString()),
                            new Action<JObject>(data => { Console.WriteLine("Ground layer Deleted!"); }));
                    }));
            }));
        }

        #endregion

        #region Code for the terrain

        /*
         * This method creates a terrain to be used
         */
        public void CreateTerrain(string texture, string normal)
        {
            connector.SendPacket(Terrain.Add(new int[] {256, 256}, CreateHeightMap()),
                new Action<JObject>(data => { CreateTerrainTexture(texture, normal); }));
        }

        /*
         * This method also creates a heightmap for the terrain.
         */
        public double[] CreateHeightMap()
        {
            double[] heightMap = new double[65536];
            Random random = new Random();

            //Initializes the heightmap for the scene
            for (int i = 0; i < 256; i++)
            {
                for (int j = 0; j < 256; j++)
                {
                    if ((i == 0 || i == 255) || (j == 0 || j == 255))
                    {
                        heightMap[(i * 255) + j] = 0;
                    }
                    else
                    {
                        heightMap[(i * 255) + j] = random.NextDouble() / 8;
                    }
                }
            }

            return heightMap;
        }

        /*
         * This method sets a texture on the terrain to make it visible.
         */
        private void CreateTerrainTexture(string texture, string normal)
        {
            string uuid = "";
            this.connector.SendPacket(Node.AddTerrain("groundPlane", null,
                    new TransformComponent(-128, 0, -128, 1, 0, 0, 0), true),
                new Action<JObject>(data =>
                {
                    uuid = data["data"]["data"]["uuid"].ToString();
                    this.connector.SendPacket(Node.AddLayer(uuid,
                            GetTextures($"terrain/{texture}"),
                            GetTextures($"terrain/{normal}"), 0, 10, 0.2),
                        new Action<JObject>(data => { Console.WriteLine("Terrain texture has been Added!"); }));
                }));
        }

        #endregion

        #region Route code

        /*
         * This method creates the route that the bike in the VR will follow.
         */
        public void CreateRoute(RouteData[] routeData)
        {
            this.connector.SendPacket(Route.Add(routeData), new Action<JObject>(data =>
            {
                Console.WriteLine($"Response add: {data}");
                this.connector.SendPacket(Route.ShowRoute(true),
                    new Action<JObject>(data => { Console.WriteLine($"Response show: {data}"); }));
                string roadID = data["data"]["data"]["uuid"].ToString();
                AddRoad(roadID);

                this.connector.SendPacket(Node.AddModel("Fiets",
                        new TransformComponent(1, -1, 1, 1, 0, 0, 0),
                        new ModelComponent(GetModelObjects("bike/bike.fbx"), true, false, "")),
                    new Action<JObject>(data =>
                    {
                        this.connector.SendPacket(Route.Follow(roadID, data["data"]["data"]["uuid"].ToString(), 1, -1,
                                "XZ", 1, false,
                                new int[] {0, 0, 0}, new int[] {0, 0, 0}),
                            new Action<JObject>(data => { Console.WriteLine("Following the set route!"); }));
                    }));
            }));
        }

        /*
         * This method creates the road texture 
         */
        private void AddRoad(string uuid)
        {
            this.connector.SendPacket(Road.AddRoad(uuid,
                    GetTextures("tarmac_diffuse.png"),
                    GetTextures("tarmac_normal.png"),
                    GetTextures("tarmax_specular.png"), 0),
                new Action<JObject>(data => { Console.WriteLine($"Response show: {data}"); }));
        }

        #endregion

        /*
         * This method is used to spawn in models such as: bikes, trees and/or cars.
         */
        public void CreateObject(string desiredModel)
        {
            this.connector.SendPacket(
                Node.AddModel(desiredModel, new TransformComponent(2, 2, 2, 1, 0, 0, 0),
                    new ModelComponent(GetModelObjects(desiredModel), true, false, "")),
                new Action<JObject>(data => { Console.WriteLine("Desired object has been created!"); }));
        }

        #region Getters for Objects and textures

        /*
         * This method gets the models inside of the data/networkEngine/models directory.
         */
        private string GetModelObjects(string objectname)
        {
            return $"data/NetworkEngine/models/{objectname}";
        }

        /*
         * This method gets the textures inside of the data/networkEngine/textures directory.
         */
        private string GetTextures(string textureName)
        {
            return $"data/NetworkEngine/textures/{textureName}";
        }

        private string GetSkyBox(string skyboxTexture)
        {
            return $"data/NetworkEngine/textures/SkyBoxes/clouds/{skyboxTexture}";
        }

        #endregion
    }
}