using System;
using System.Threading;
using System.Threading.Tasks;
using BikeApp.command.scene;
using BikeApp.connections;
using BikeApp.connections.bluetooth;
using BikeApp.data;
using BikeApp.data.components;
using Newtonsoft.Json.Linq;

// ReSharper disable LocalizableElement

namespace BikeApp.vr_environment
{
    internal class CommandCenter
    {
        private string panelUuid;
        private string bikeUuid;
        private VpnConnector connector;
        private ConnectorOption updateValues;
        public bool Running { get; set; }
        public Task UpdateThread { get; set; }


        /**
         * Controller for managing construction and management of commands.
         * TODO Trees everywhere!
         * TODO Mount the camera to the bike or object.
         */
        public CommandCenter(VpnConnector vpnConnector)
        {
            connector = vpnConnector;
            panelUuid = "";
            bikeUuid = "";
        }

        public void SetConnectorOption(ConnectorOption connectorOption)
        {
            updateValues = connectorOption;
        }

        public void SetSpeed(float speed)
        {
            updateValues.Speed = speed;
        }

        public void AttachCamera()
        {
            connector.SendPacket(Node.Find("Camera"),
                new Action<JObject>(cameraData =>
                {
                    connector.SendPacket(Node.Find("Bike"),
                        new Action<JObject>(bikeData =>
                        {
                            var component = new TransformComponent(
                                new double[] {5, 0, 0},
                                1, new double[] {0, 90, 0});

                            connector.SendPacket(Node.Update(
                                    cameraData["data"]?["data"]?[0]?["uuid"]?.ToString(),
                                    bikeData["data"]?["data"]?[0]?["uuid"]?.ToString(),
                                    component),
                                new Action<JObject>(data => { }));
                        }));
                }));
        }

        /*
         * This method creates a preset VR environment
         */

        public void PresetOne()
        {
            ResetScene();
            CreateTerrain("snow_grass_d.jpg", "snow_grass_n.jpg");

            var routeData = new RouteData[7];
            // Defining route
            routeData[0] = new RouteData(new[] {0, 0, 0}, new double[] {30, 0, -20});
            routeData[1] = new RouteData(new[] {20, 0, 0}, new double[] {0, 0, 20});
            routeData[2] = new RouteData(new[] {20, 0, 50}, new double[] {0, 0, 20});
            routeData[3] = new RouteData(new[] {-10, 0, 30}, new double[] {50, 0, -20});
            routeData[4] = new RouteData(new[] {-30, 0, 10}, new double[] {9, 0, -15});
            routeData[5] = new RouteData(new[] {-20, 0, 0}, new double[] {0, 0, -20});
            routeData[6] = new RouteData(new[] {-10, 0, -10}, new double[] {9, 0, -20});

            CreateRoute(routeData);
            SetTime(SkyBoxTime.Morning);

            Running = true;
            UpdateThread = new Task(Update);
            UpdateThread.Start();
        }

        #region Updatables

        private void Update()
        {
            while (Running)
            {
                Thread.Sleep(4000);
                UpdatePanel(panelUuid, updateValues.Speed, updateValues.HeartRate, updateValues.Resistance);
                UpdateBike(bikeUuid, updateValues.Speed);
            }
        }

        private void UpdateBike(string uuid, float speed)
        {
            connector.SendPacket(Route.Speed(uuid, speed),
                new Action<JObject>(data => { Console.WriteLine("Bike speed has been updated!"); }));
        }

        #endregion

        #region Scene Code

        private void GetScene()
        {
            connector.SendPacket(Scene.Get(),
                new Action<JObject>(data => { Console.WriteLine("Scene data: {0}", data); }));
        }

        /*
         * This method deletes the original groundplane and resets the entire scene. 
         */
        public void ResetScene()
        {
            connector.SendPacket(Scene.Reset(), new Action<JObject>(data =>
            {
                Console.WriteLine("Scene has been reset!");
                connector.SendPacket(Node.Find("GroundPlane"),
                    new Action<JObject>(nodeData =>
                    {
                        connector.SendPacket(Node.Delete(nodeData["data"]?["data"]?[0]?["uuid"]?.ToString()),
                            new Action<JObject>(deleteData => { Console.WriteLine("Ground layer Deleted!"); }));
                    }));
            }));
        }

        private void SetTime(SkyBoxTime time)
        {
            switch (time)
            {
                case SkyBoxTime.Morning:
                    connector.SendPacket(Skybox.SetTime(9), new Action<JObject>(data =>
                    {
                        connector.SendPacket(Skybox.Update("static",
                                GetSkyBox("bluecloud_rt.jpg"), GetSkyBox("bluecloud_lf.jpg"),
                                GetSkyBox("bluecloud_up.jpg"),
                                GetSkyBox("bluecloud_dn.jpg"), GetSkyBox("bluecloud_bk.jpg"),
                                GetSkyBox("bluecloud_ft.jpg")),
                            new Action<JObject>(morningData => { Console.WriteLine("It's daytime!"); }));
                    }));
                    break;

                case SkyBoxTime.Afternoon:
                    connector.SendPacket(Skybox.SetTime(12), new Action<JObject>(data =>
                    {
                        connector.SendPacket(Skybox.Update("static",
                                GetSkyBox("graycloud_rt.jpg"), GetSkyBox("graycloud_lf.jpg"),
                                GetSkyBox("graycloud_up.jpg"),
                                GetSkyBox("graycloud_dn.jpg"), GetSkyBox("graycloud_bk.jpg"),
                                GetSkyBox("graycloud_ft.jpg")),
                            new Action<JObject>(afternoonData => { Console.WriteLine("It's the afternoon!"); }));
                    }));
                    break;

                case SkyBoxTime.Evening:
                    connector.SendPacket(Skybox.SetTime(20), new Action<JObject>(data =>
                    {
                        connector.SendPacket(Skybox.Update("static",
                                GetSkyBox("yellowcloud_rt.jpg"), GetSkyBox("yellowcloud_lf.jpg"),
                                GetSkyBox("yellowcloud_up.jpg"),
                                GetSkyBox("yellowcloud_dn.jpg"), GetSkyBox("yellowcloud_bk.jpg"),
                                GetSkyBox("yellowcloud_ft.jpg")),
                            new Action<JObject>(eveningData => { Console.WriteLine("It's in the evening!"); }));
                    }));
                    break;

                case SkyBoxTime.Night:
                    connector.SendPacket(Skybox.SetTime(22), new Action<JObject>(data =>
                    {
                        connector.SendPacket(Skybox.Update("static",
                                GetSkyBox("graycloud_rt.jpg"), GetSkyBox("graycloud_lf.jpg"),
                                GetSkyBox("graycloud_up.jpg"),
                                GetSkyBox("graycloud_dn.jpg"), GetSkyBox("graycloud_bk.jpg"),
                                GetSkyBox("graycloud_ft.jpg")),
                            new Action<JObject>(nightData => { Console.WriteLine("It's nightTime!"); }));
                    }));
                    break;
            }
        }

        #endregion

        #region Code for the terrain

        /*
         * This method creates a terrain to be used
         */
        private void CreateTerrain(string texture, string normal)
        {
            connector.SendPacket(Terrain.Add(new[] {256, 256}, CreateHeightMap()),
                new Action<JObject>(data =>
                {
                    Console.WriteLine("Reached the terrain skeleton!");
                    CreateTerrainTexture(texture, normal);
                }));
        }

        /*
         * This method also creates a heightmap for the terrain.
         */
        private double[] CreateHeightMap()
        {
            var heightMap = new double[65536];
            var random = new Random();

            //Initializes the heightmap for the scene
            for (var i = 0; i < 256; i++)
            {
                for (var j = 0; j < 256; j++)
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
            string uuid;
            connector.SendPacket(Node.AddTerrain("groundPlane", null,
                    new TransformComponent(-128, 0, -128, 1, 0, 0, 0), true),
                new Action<JObject>(data =>
                {
                    uuid = data["data"]?["data"]?["uuid"]?.ToString();
                    connector.SendPacket(Node.AddLayer(uuid,
                            GetTextures($"terrain/{texture}"),
                            GetTextures($"terrain/{normal}"), 0, 10, 0.2),
                        new Action<JObject>(nodeData =>
                        {
                            Console.WriteLine("Terrain texture has been Added!");
                            CreateTrees();
                        }));
                }));
        }

        /*
         * TODO Around the road there should be trees, not inside or on the road.
         * TODO Only one part of the land gets filled with trees, do that for all of the areas inside of the terrain.
         * This method creates the trees for the VR environment.
         */
        private void CreateTrees()
        {
            var random = new Random();
            for (var i = 0; i < 100; i++)
            {
                connector.SendPacket(Node.AddModel(("Tree" + i),
                        "", new TransformComponent(random.NextDouble() * 100, 0, random.NextDouble() * 100, 1, 0, 0, 0),
                        new ModelComponent(GetModelObjects("trees/fantasy/tree6.obj"), false, false, "")),
                    new Action<JObject>(data =>
                    {
                        var uuid = data["data"]?["data"]?["uuid"]?.ToString();
                        connector.SendPacket(
                            Node.AddLayer(uuid, GetModelObjects("trees/fantasy/Tree_10_Tree.png"), "", 0, 10, 0.2),
                            new Action<JObject>(treeData => { }));
                    }));
            }
        }

        #endregion

        #region Route code

        /*
         * This method creates the route that the bike in the VR will follow.
         */
        public void CreateRoute(RouteData[] routeData)
        {
            connector.SendPacket(Route.Add(routeData), new Action<JObject>(data =>
            {
                Console.WriteLine($"Response add: {data}");
                connector.SendPacket(Route.ShowRoute(false),
                    new Action<JObject>(dataRoute => { Console.WriteLine("Route skeleton had been made"); }));
                var roadId = data["data"]?["data"]?["uuid"]?.ToString();
                AddRoad(roadId);

                connector.SendPacket(Node.AddModel("Bike", null,
                        new TransformComponent(1, -1, 1, 1, 0, 45, 0),
                        new ModelComponent(GetModelObjects("bike/bike.fbx"), true, false, "")),
                    new Action<JObject>(bikeData =>
                    {
                        bikeUuid = bikeData["data"]?["data"]?["uuid"]?.ToString();
                        CreatePanel(bikeUuid);
                        connector.SendPacket(Route.Follow(roadId, bikeData["data"]?["data"]?["uuid"]?.ToString(),
                                updateValues.Speed / 3,
                                -1,
                                "XZ", 1, false,
                                new[] {0, 0, 0}, new[] {0, 0, 0}),
                            new Action<JObject>(e => { Console.WriteLine("Following the set route!"); }));
                    }));
            }));
        }

        /*
         * This method creates the road texture 
         */
        private void AddRoad(string uuid)
        {
            connector.SendPacket(Road.AddRoad(uuid,
                    GetTextures("tarmac_diffuse.png"),
                    GetTextures("tarmac_normal.png"),
                    GetTextures("tarmac_specular.png"), 0),
                new Action<JObject>(data => { Console.WriteLine("Bike now following a textured road!"); }));
        }

        #endregion

        #region Panel code

        /*
         * This method creates a panel.
         */
        private void CreatePanel(string nodeUuid)
        {
            connector.SendPacket(Node.AddPanel("Panel", nodeUuid,
                    new PanelComponent(0.5, 0.5, 512, 512, 1, 1, 1, 1, false),
                    new TransformComponent(-0.3, 1.2, 0.01, 0.45, -20, 90, 0)),
                new Action<JObject>(data => { panelUuid = data["data"]?["data"]?["uuid"]?.ToString(); }));
        }

        /*
         * This method updates the panel through a panel.swap,
         * and then draws the current values onto the panel.
         */
        public void UpdatePanel(string uuid, float speed, int heartRate, float resistance)
        {
            ClearPanel(uuid);
            DrawValues(uuid, speed, heartRate, resistance);
        }

        /*
         * This method clears the current panel that is being displayed.
         */
        private void ClearPanel(string uuid)
        {
            connector.SendPacket(Panel.Clear(uuid),
                new Action<JObject>(data => { }));
        }

        private void DrawValues(string uuid, double speed, double heartRate, double resistance)
        {
            connector.SendPacket(Panel.DrawText(uuid,
                    $"Current speed: {String.Format("{0:0.00}", speed)}m/s", new[] {100, 100}, 32, new[] {0, 0, 0, 1}, "Arial"),
                new Action<JObject>(data =>
                {
                    connector.SendPacket(Panel.DrawText(uuid, $"Heart rate: {heartRate}bpm",
                        new[] {100, 200}, 32, new[] {0, 0, 0, 1}, "Arial"), new Action<JObject>(d =>
                    {
                        connector.SendPacket(Panel.DrawText(uuid,
                                $"Current resistance: {resistance}%",
                                new[] {100, 300}, 32, new[] {0, 0, 0, 1}, "Arial"),
                            new Action<JObject>(e =>
                            {
                                connector.SendPacket(Panel.Swap(uuid),
                                    new Action<JObject>(
                                        o => { }));
                            }));
                    }));
                }));
        }

        #endregion

        #region Kinda useful code

        /*
         * This method is used to spawn in models such as: bikes, trees and/or cars.
         */
        private void CreateObject(string desiredModel, string modelObject)
        {
            connector.SendPacket(
                Node.AddModel(desiredModel, "",
                    new TransformComponent(2, 2, 2, 1, 0, 0, 0),
                    new ModelComponent(GetModelObjects(modelObject), true, false, "")),
                new Action<JObject>(parentData =>
                {
                    var uuid = parentData["data"]?["data"]?["uuid"]?.ToString();
                    CreatePanel(uuid);
                }));
        }

        #endregion

        #region Getters for Objects, textures and fonts

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