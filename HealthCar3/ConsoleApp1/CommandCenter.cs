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
         */
        public CommandCenter(VpnConnector vpn)
        {
            this.connector = vpn;
        }

        public void PresetOne(int time)
        {
            ResetScene();
            CreateTerrain();
            CreateRoute();
            SetTime(time);
        }

        public void SetTime(double time)
        {
            this.connector.SendPacket(Skybox.SetTime(time), new Action<JObject>(data =>
           {
               Console.WriteLine("Time set");
               if (time < 12 && time >= 5)
               {
                   this.connector.SendPacket(Skybox.Update("static",
                       GetSkyBox("bluecloud_rt.jpg"), GetSkyBox("bluecloud_lf.jpg"), GetSkyBox("bluecloud_up.jpg"),
                       GetSkyBox("bluecloud_dn.jpg"), GetSkyBox("bluecloud_bk.jpg"), GetSkyBox("bluecloud_ft.jpg")), new Action<JObject>(data =>
                       {
                           Console.WriteLine("It's daytime!");
                       }));
               }
               else if (time < 18)
               {
                   this.connector.SendPacket(Skybox.Update("static",
                       GetSkyBox("graycloud_rt.jpg"), GetSkyBox("graycloud_lf.jpg"), GetSkyBox("graycloud_up.jpg"),
                       GetSkyBox("graycloud_dn.jpg"), GetSkyBox("graycloud_bk.jpg"), GetSkyBox("graycloud_ft.jpg")), new Action<JObject>(data =>
                       {
                           Console.WriteLine("It's the afternoon!");
                       }));
               }
               else if (time < 21)
               {
                   this.connector.SendPacket(Skybox.Update("static",
                       GetSkyBox("yellowcloud_rt.jpg"), GetSkyBox("yellowcloud_lf.jpg"), GetSkyBox("yellowcloud_up.jpg"),
                       GetSkyBox("yellowcloud_dn.jpg"), GetSkyBox("yellowcloud_bk.jpg"), GetSkyBox("yellowcloud_ft.jpg")), new Action<JObject>(data =>
                       {
                           Console.WriteLine("It's in the evening!");
                       }));
               }
               else
               {
                   this.connector.SendPacket(Skybox.Update("static",
                        GetSkyBox("graycloud_rt.jpg"), GetSkyBox("graycloud_lf.jpg"), GetSkyBox("graycloud_up.jpg"),
                        GetSkyBox("graycloud_dn.jpg"), GetSkyBox("graycloud_bk.jpg"), GetSkyBox("graycloud_ft.jpg")), new Action<JObject>(data =>
                        {
                            Console.WriteLine("It's nightTime!");
                        }));
               }
           }));
        }

        public void GetScene()
        {
            this.connector.SendPacket(Scene.Get(), new Action<JObject>(data =>
            {
                Console.WriteLine("Scene data: {0}", data);
            }));
        }

        public void ResetScene()
        {
            this.connector.SendPacket(Scene.Reset(), new Action<JObject>(data =>
            {
                Console.WriteLine("Scene has been reset!");
                this.connector.SendPacket(Node.Find("GroundPlane"), new Action<JObject>(data =>
                {
                    this.connector.SendPacket(Node.Delete(data["data"]["data"][0]["uuid"].ToString()), new Action<JObject>(data =>
                    {
                        Console.WriteLine("Ground layer Deleted!");
                    }));

                }));
            }));
        }

        public void CreateTerrain()
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
                        heightMap[(i * 255) + j] = random.NextDouble() / 4;
                    }
                }
            }

            this.connector.SendPacket(Terrain.Add(new int[] { 256, 256 }, heightMap), new Action<JObject>(data =>
            {
                CreateTerrainTexture();
            }));
        }

        private void CreateTerrainTexture()
        {
            this.connector.SendPacket(Node.AddTerrain("groundPlane", null, new TransformComponent(-128, -1, -128, 1, 0, 0, 0), true), new Action<JObject>(data =>
            {
                string uuid = data["data"]["data"]["uuid"].ToString();
                this.connector.SendPacket(Node.AddLayer(uuid, GetTextures("terrain/desert_cracksv_d.jpg"), GetTextures("terrain/desert_cracksv_s.jpg"), 0, 10, 0.2), new Action<JObject>(data =>
                {
                   Console.WriteLine("Texture Data: {0}", data);
                }));
            }));
        }

        public void CreateRoute()
        {
            RouteData[] routeData = new RouteData[4];
            // Defining route
            routeData[0] = new RouteData(new int[] { 1, -1, 1 }, new int[] { 1, -1, 0 });
            routeData[1] = new RouteData(new int[] { 11, -1, 1 }, new int[] { 0, -1, 1 });
            routeData[2] = new RouteData(new int[] { 11, -1, 11 }, new int[] { -1, -1, 0 });
            routeData[3] = new RouteData(new int[] { 1, -1, 11 }, new int[] { 0, -1, -1 });

            this.connector.SendPacket(Route.Add(routeData), new Action<JObject>(data =>
            {
                Console.WriteLine($"Response add: {data}");
                this.connector.SendPacket(Route.ShowRoute(true), new Action<JObject>(data =>
                {
                    Console.WriteLine($"Response show: {data}");
                }));
                string roadID = data["data"]["data"]["uuid"].ToString();
                AddRoad(data["data"]["data"]["uuid"].ToString());
                this.connector.SendPacket(Node.AddModel("Fiets", new TransformComponent(1, 0.2, 1, 1, 0, 0, 0), new ModelComponent(GetModelObjects("bike/bike.fbx"), true, false, "")), new Action<JObject>(data =>
                {
                    this.connector.SendPacket(Route.Follow(roadID, data["data"]["data"]["uuid"].ToString(), 1, 0, "XZ", 1, false, new int[] { 0, 0, 0 }, new int[] { 0, -1, 0 }), new Action<JObject>(data =>
                          {
                              Console.WriteLine("Following the route!");
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

        private string GetModelObjects(string objectname)
        {
            return $"data/NetworkEngine/models/{objectname}";
        }

        private string GetTextures(string textureName)
        {
            return $"data/NetworkEngine/textures/{textureName}";
        }

        private string GetSkyBox(string skyboxTexture)
        {
            return $"data/NetworkEngine/textures/SkyBoxes/clouds/{skyboxTexture}";
        }
    }
}
