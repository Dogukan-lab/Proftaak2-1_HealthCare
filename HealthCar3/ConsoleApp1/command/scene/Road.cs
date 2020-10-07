namespace ConsoleApp1.command.scene
{
    static class Road
    {
        static string prefix = "scene/road/";

        /**
         * This method adds a route to the vr system
         */
        public static dynamic AddRoad(string routeId, string diffuse, string normal, string specular, double heightoffset)
        {
            dynamic packetData = new
            {
                route = routeId,
                diffuse = diffuse,
                normal = normal,
                specular = specular,
                heightoffset = heightoffset
            };
            return CommandUtils.Wrap(packetData, prefix + "add");
        }

        /**
         * This method updates the route that has been set.
         */
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
            return CommandUtils.Wrap(packetData, prefix + "update");
        }
    }
}
