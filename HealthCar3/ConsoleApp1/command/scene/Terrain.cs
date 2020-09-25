using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp1.command.scene
{
    static class Terrain
    {
        static string prefix = "scene/terrain/";

        /**
         * This method adds a skeleton terrain for the terrain textures.
         */
        public static dynamic Add(int[] size, int[] height)
        {
            dynamic packetData = new
            {
                size = size,
                height = height
            };
            return CommandUtils.Wrap(packetData, prefix + "add");
        }

        /**
         * This method updates the terrain
         */
        public static dynamic Update()
        {
            dynamic packetData = new
            {

            };
            return CommandUtils.Wrap(packetData, prefix + "update");
        }

        /**
         * This method deletes the terrain
         */
        public static dynamic Delete()
        {
            dynamic packetData = new
            {

            };
            return CommandUtils.Wrap(packetData, prefix + "delete");
        }

        /**
         * This method gets the height map of the terrain
         */
        public static dynamic GetHeight(int[] position, int[,] positions)
        {
            dynamic packetData = new
            {
                position = position,
                positions = positions
            };
            return CommandUtils.Wrap(packetData, prefix + "update");
        }
    }
}
