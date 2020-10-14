using System;
using System.Collections.Generic;
using System.Text;

namespace BikeApp.command.scene
{
    static  class Scene
    {

        static string prefix = "scene/";

        /**
         * This method gets the data inside of the scene in json format
         */
        public static dynamic Get()
        {
            dynamic packetData = new
            {
            };
            return CommandUtils.Wrap(packetData, prefix + "get");
        }

        /**
         * This method resets the scene to it's default state
         */
        public static dynamic Reset()
        {
            dynamic packetData = new
            {
            };
            return CommandUtils.Wrap(packetData, prefix + "reset");
        }

        /**
         * This method saves the scene in a designated file.
         */
        public static dynamic Save(string filename)
        {
            dynamic packetData = new
            {
                filename = filename,
                overwrite = true
            };
            return CommandUtils.Wrap(packetData, prefix + "save");
        }

        /**
         * This method loads the scene from the save file
         */
        public static dynamic Load(string filename)
        {
            dynamic packetData = new
            {
                filename = filename,
            };
            return CommandUtils.Wrap(packetData, prefix + "load");
        }

        /**
         * This method casts a ray through the scene, and returns an array of collision points.
         */
        public static dynamic Raycast(int[] start, int[] direction, bool physics)
        {
            dynamic packetData = new
            {
                start = start,
                direction = direction,
                physics = physics
            };
            return CommandUtils.Wrap(packetData, prefix + "raycast");
        }

    }
}
