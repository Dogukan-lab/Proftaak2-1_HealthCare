namespace BikeApp.command.scene
{
    public static class Road
    {
        private const string Prefix = "scene/road/";

        /**
         * This method adds a route to the vr system
         */
        public static dynamic AddRoad(string routeId, string diffusePng, string normalPng, string specularPng,
            double heightOffset)
        {
            dynamic packetData = new
            {
                route = routeId,
                diffuse = diffusePng,
                normal = normalPng,
                specular = specularPng,
                heightoffset = heightOffset
            };
            return CommandUtils.Wrap(packetData, Prefix + "add");
        }

        /**
         * This method updates the route that has been set.
         */
        public static dynamic Update(string roadId, string routeId, string diffusePng, string normalPng,
            string specularPng, double heightOffset)
        {
            dynamic packetData = new
            {
                id = roadId,
                route = routeId,
                diffuse = diffusePng,
                normal = normalPng,
                specular = specularPng,
                heightoffset = heightOffset
            };
            return CommandUtils.Wrap(packetData, Prefix + "update");
        }
    }
}