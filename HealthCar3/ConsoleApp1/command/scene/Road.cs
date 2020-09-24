using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace ConsoleApp1.command.scene
{
    static class Road
    {
        static string prefix = "scene/road/";

        private static dynamic Wrap(dynamic roadData, string id)
        {
            dynamic packet = new
            {
                id = id,
                data = roadData
            };
            return packet;
        }

        public static dynamic AddRoute(string routeId, string diffuse, string normal, string specular, double heightoffset)
        {
            dynamic packetData = new
            {
                route = routeId,
                diffuse = diffuse,
                normal = normal,
                specular = specular,
                heightoffset = heightoffset
            };
            return Wrap(packetData, prefix + "add");
        }

        public static dynamic Update(string roadId, string routeId, string diffuse, string normal, string specular, double heightoffset)
        {
            dynamic packetData = new
            {
                id = roadId,
                route = routeId,
                diffuse = diffuse,
                normal = normal,
                specular = specular,
                heightoffset = heightoffset
            };
            return Wrap(packetData, prefix + "update");
        }
    }
}
