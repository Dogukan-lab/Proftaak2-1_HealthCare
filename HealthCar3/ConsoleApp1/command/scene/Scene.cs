using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp1.command.scene
{
    static  class Scene
    {

        static string prefix = "scene/road/";

        /**
         * This method adds a scene to the vr system
         */
        public static dynamic GetScene()
        {
            dynamic packetData = new
            {
            };
            return SceneUtils.Wrap(packetData, prefix + "get");
        }

        public static dynamic ResetScene()
        {
            dynamic packetData = new
            {
            };
            return SceneUtils.Wrap(packetData, prefix + "reset");
        }

        public static dynamic SaveScene(string filename)
        {
            dynamic packetData = new
            {
                filename = filename,
                overwrite = true
            };
            return SceneUtils.Wrap(packetData, prefix + "save");
        }

        public static dynamic LoadScene(string filename)
        {
            dynamic packetData = new
            {
                filename = filename,
            };
            return SceneUtils.Wrap(packetData, prefix + "load");
        }

        public static dynamic RaycastScene(int[] start, int[] direction, bool physics)
        {
            dynamic packetData = new
            {
                start = start,
                direction = direction,
                physics = physics
            };
            return SceneUtils.Wrap(packetData, prefix + "raycast");
        }

    }
}
