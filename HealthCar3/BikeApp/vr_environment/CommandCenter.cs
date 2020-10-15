using System;
using BikeApp.command.scene;
using BikeApp.connections;
using BikeApp.data;
using BikeApp.data.components;
using Newtonsoft.Json.Linq;

// ReSharper disable StringLiteralTypo

namespace BikeApp.vr_environment
{
    /*
     * Controller for managing construction and management of commands.
     * TODO Trees everywhere!
     * TODO Mount the camera to the bike or object.
     * TODO make a panelUuid so that the values can be updated on the panel.
     */
    internal class CommandCenter
    {
        private readonly VpnConnector connector;

        public CommandCenter(VpnConnector vpn)
        {
            connector = vpn;
        }

        /*
         * This method creates a preset VR environment
         */
        public void PresetOne()
        {
            ResetScene();
            CreateTerrain("snow_grass_d.jpg", "snow_grass_n.jpg");

            RouteData[] routeData = new RouteData[7];
            // Defining the route
            routeData[0] = new RouteData(new[] {0, 0, 0}, new[] {1, 0, 0});
            routeData[1] = new RouteData(new[] {20, 0, 0}, new[] {0, 0, 1});
            routeData[2] = new RouteData(new[] {20, 0, 50}, new[] {-1, 0, 0});
            routeData[3] = new RouteData(new[] {-10, 0, 30}, new[] {-1, 0, 0});
            routeData[4] = new RouteData(new[] {-30, 0, 10}, new[] {0, 0, 0});
            routeData[5] = new RouteData(new[] {-20, 0, 0}, new[] {0, 0, 0});
            routeData[6] = new RouteData(new[] {-10, 0, -10}, new[] {1, 0, 0});

            CreateRoute(routeData, 3);
            SetTime(SkyBoxTime.Morning);
        }

        #region Scene Code

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
                            new Action<JObject>(updateData => { Console.WriteLine(@"It's daytime!"); }));
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
                            new Action<JObject>(updateData => { Console.WriteLine(@"It's the afternoon!"); }));
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
                            new Action<JObject>(updateData => { Console.WriteLine(@"It's in the evening!"); }));
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
                            new Action<JObject>(updateData => { Console.WriteLine(@"It's nightTime!"); }));
                    }));
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(time), time, null);
            }
        }

        /*
         * Gets the first scene
         * TODO use this to get the camera, and then bind it to the bike.
         */
        public void GetScene()
        {
            connector.SendPacket(Scene.Get(),
                new Action<JObject>(data => { Console.WriteLine(@"Scene data: {0}", data); }));
        }

        /*
         * This method deletes the original ground plane and resets the entire scene. 
         */
        private void ResetScene()
        {
            connector.SendPacket(Scene.Reset(), new Action<JObject>(data =>
            {
                Console.WriteLine(@"Scene has been reset!");
                connector.SendPacket(Node.Find("GroundPlane"),
                    new Action<JObject>(nodeData =>
                    {
                        connector.SendPacket(Node.Delete(data["data"]?["data"]?[0]?["uuid"]?.ToString()),
                            new Action<JObject>(deleteData => { Console.WriteLine(@"Ground layer Deleted!"); }));
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
            connector.SendPacket(Terrain.Add(new[] {256, 256}, CreateHeightMap()),
                new Action<JObject>(data =>
                {
                    Console.WriteLine(@"Reached the terrain skeleton!");
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
            this.connector.SendPacket(Node.AddTerrain("groundPlane", null,
                    new TransformComponent(-128, 0, -128, 1, 0, 0, 0), true),
                new Action<JObject>(data =>
                {
                    uuid = data["data"]?["data"]?["uuid"]?.ToString();
                    this.connector.SendPacket(Node.AddLayer(uuid,
                            GetTextures($"terrain/{texture}"),
                            GetTextures($"terrain/{normal}"), 0, 10, 0.2),
                        new Action<JObject>(layerData =>
                        {
                            Console.WriteLine(@"Terrain texture has been Added!");
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
                        new TransformComponent(random.NextDouble() * 100, 0, random.NextDouble() * 100, 1, 0, 0, 0),
                        new ModelComponent(GetModelObjects("trees/fantasy/tree6.obj"), false, false, "")),
                    new Action<JObject>(data =>
                    {
                        var uuid = data["data"]?["data"]?["uuid"]?.ToString();
                        connector.SendPacket(
                            Node.AddLayer(uuid, GetModelObjects("trees/fantasy/Tree_10_Tree.png"), "", 0, 10, 0.2),
                            new Action<JObject>(layerData => { Console.WriteLine(@"Tree texture has been added!"); }));
                    }));
            }
        }

        #endregion

        #region Route code

        /*
         * This method creates the route that the bike in the VR will follow.
         */
        private void CreateRoute(RouteData[] routeData, int speed)
        {
            connector.SendPacket(Route.Add(routeData), new Action<JObject>(data =>
            {
                connector.SendPacket(Route.ShowRoute(true),
                    new Action<JObject>(showRoute => { Console.WriteLine(@"Route skeleton had been made"); }));
                var roadId = data["data"]?["data"]?["uuid"]?.ToString();
                AddRoad(roadId);

                connector.SendPacket(Node.AddModel("Bike",
                        new TransformComponent(1, -1, 1, 1, 0, 0, 0),
                        new ModelComponent(GetModelObjects("bike/bike.fbx"), true, false, "")),
                    new Action<JObject>(modelData =>
                    {
                        string parent = data["data"]?["data"]?["uuid"]?.ToString();
                        CreatePanel(parent);
                        connector.SendPacket(Route.Follow(roadId, data["data"]?["data"]?["uuid"]?.ToString(),
                                speed, -1, "XZ", 1, false,
                                new[] {0, 0, 0}, new[] {0, 0, 0}),
                            new Action<JObject>(followData => { Console.WriteLine(@"Following the set route!"); }));
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
                new Action<JObject>(data => { Console.WriteLine(@"Bike now following a textured road!"); }));
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
                    new TransformComponent(-0.4, 1.3, 0.01, 1, -20, 90, 0)),
                new Action<JObject>(data =>
                {
                    string uuid = data["data"]?["data"]?["uuid"]?.ToString();
                    UpdatePanel(uuid);
                }));
        }

        /*
         * This method clears the current panel that is being displayed.
         */
        private void ClearPanel(string uuid)
        {
            connector.SendPacket(Panel.Clear(uuid),
                new Action<JObject>(data => { Console.WriteLine(@"Panel has been cleared!"); }));
        }

        /*
         * This method updates the panel through a panel.swap,
         * and then draws the current values onto the panel.
         */
        private void UpdatePanel(string uuid)
        {
            ClearPanel(uuid);
            DrawValues(uuid, 0, 100, 20);
        }

        private void DrawValues(string uuid, double speed, double heartRate, double resistance)
        {
            connector.SendPacket(Panel.DrawText(uuid,
                    $"Current speed: {speed}m/s", new double[] {100, 100}, 32, new[] {0, 0, 0, 1}, "Arial"),
                new Action<JObject>(data =>
                {
                    this.connector.SendPacket(Panel.DrawText(uuid, $"Heart rate: {heartRate}bpm",
                            new double[] {100, 200}, 32, new[] {0, 0, 0, 1}, "Arial"),
                        new Action<JObject>(lineOne =>
                        {
                            connector.SendPacket(Panel.DrawText(uuid,
                                    $"Current resistance: {resistance}%",
                                    new double[] {100, 300}, 32, new[] {0, 0, 0, 1}, "Arial"),
                                new Action<JObject>(lineTwo =>
                                {
                                    connector.SendPacket(Panel.Swap(uuid),
                                        new Action<JObject>(
                                            panelSwap => { Console.WriteLine(@"Panel swap data: {0}", data); }));
                                }));
                        }));
                }));
        }

        #endregion

        /*
         * This method is used to spawn in models such as: bikes, trees and/or cars.
         */
        public void CreateObject(string desiredModel)
        {
            connector.SendPacket(Node.AddModel(desiredModel,
                    new TransformComponent(2, 2, 2, 1, 0, 0, 0),
                    new ModelComponent(GetModelObjects(desiredModel), true, false, "")),
                new Action<JObject>(data => { Console.WriteLine(@"Desired object has been created!"); }));
        }

        #region Getters for Objects, textures and fonts

        /*
         * This method gets the models inside of the data/networkEngine/models directory.
         */
        private string GetModelObjects(string objectName)
        {
            return $"data/NetworkEngine/models/{objectName}";
        }

        /*
         * This method gets the textures inside of the data/networkEngine/textures directory.
         */
        private string GetTextures(string textureName)
        {
            return $"data/NetworkEngine/textures/{textureName}";
        }

        /*
         * This method gets the textures inside of the data/networkEngine/textures/SkyBox directory.
         */
        private string GetSkyBox(string skyboxTexture)
        {
            return $"data/NetworkEngine/textures/SkyBoxes/clouds/{skyboxTexture}";
        }

        #endregion
    }
}