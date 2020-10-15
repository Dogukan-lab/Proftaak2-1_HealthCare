namespace BikeApp.command.scene
{
    static class Route
    {
        private const string Prefix = "route/";

        /**
         * This method adds a route to the vr system
         */
        public static dynamic Add(object[] nodes)
        {
            dynamic packetData = new { nodes = nodes };
            return CommandUtils.Wrap(packetData, Prefix + "add");
        }

        /**
         * This method updates the current route
         */
        public static dynamic UpdateRoute(string routeUuid, object[] routeNodes)
        {
            dynamic packetData = new
            {
                uuid = routeUuid,
                nodes = routeNodes
            };
            return CommandUtils.Wrap(packetData, Prefix + "update");
        }

        /**
         * This method deletes a specified route
         */
        public static dynamic Delete(string routeUuid)
        {
            dynamic packetData = new { uuid = routeUuid };
            return CommandUtils.Wrap(packetData, Prefix + "delete");
        }

        /**
         * This method makes a node follow the route
         */
        public static dynamic Follow(string routeUuid, string nodeId, double desiredSpeed, 
            double offset, string rotation, double smoothing,
            bool followHeight, int[] rotationOffset, int[] positionOffset)
        {
            dynamic packetData = new
            {
                route = routeUuid,
                node = nodeId,
                speed = desiredSpeed,
                offset = offset,
                rotate = rotation,
                smoothing = smoothing,
                followHeight = followHeight,
                rotateOffset = rotationOffset,
                positionOffset = positionOffset
            };
            return CommandUtils.Wrap(packetData, Prefix + "follow");
        }

        /**
         * This method changes the speed of a nod on a route
         */
        public static dynamic Speed(string nodeId, double desiredSpeed)
        {
            dynamic packetData = new
            {
                node = nodeId,
                speed = desiredSpeed
            };
            return CommandUtils.Wrap(packetData, Prefix + "follow/speed");
        }

        /**
         * This method shows the route with red lining.
         */
        public static dynamic ShowRoute(bool showRoute)
        {
            dynamic packetData = new { show = showRoute };
            return CommandUtils.Wrap(packetData, Prefix + "show");
        }
    }
}
