using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp1.command.scene
{
    static class Terrain
    {
        static string prefix = "scene/terrain/";

        private static dynamic wrap(dynamic terrainData, string id)
        {
            dynamic packet = new
            {
                id = id,
                data = terrainData
            };
            return packet;
        }

        public static dynamic AddTerrain(int[] size, int[] height)
        {
            dynamic packetData = new
            {
                size = size,
                height = height
            };
            return wrap(packetData, prefix + "add");
        }

        public static dynamic Update()
        {
            dynamic packetData = new
            {

            };
            return wrap(packetData, prefix + "update");
        }

        public static dynamic Delete()
        {
            dynamic packetData = new
            {

            };
            return wrap(packetData, prefix + "delete");
        }

        public static dynamic GetHeight(int[] position, int[,] positions)
        {
            dynamic packetData = new
            {
                position = position,
                positions = positions
            };
            return wrap(packetData, prefix + "update");
        }
    }
}
