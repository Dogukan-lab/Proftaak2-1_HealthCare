using System;
using System.Collections.Generic;
using System.Text;
using BikeApp.command.scene;

namespace BikeApp.command.route
{
    static class Route
    {

        static string prefix = "route/";

        /**
         * This method adds a route to the vr system
         */
        public static dynamic Add( object[] nodes)
        {
            dynamic packetData = new
            {
                nodes = nodes
            };
            return CommandUtils.Wrap(packetData, prefix + "add");
        }

        /**
         * This method updates the current route
         */
        public static dynamic UpdateRoute(string uuid, object[] nodes)
        {
            dynamic packetData = new
            {
                uuid = uuid,
                nodes = nodes
            };
            return CommandUtils.Wrap(packetData, prefix + "update");
        }

        /**
         * This method deletes a specified route
         */
        public static dynamic Delete(string uuid)
        {
            dynamic packetData = new
            {
                uuid = uuid
            };
            return CommandUtils.Wrap(packetData, prefix + "delete");
        }

        /**
         * This method makes a node follow the route
         */
        public static dynamic Follow(string routeId, string nodeId, double speed, double offset, string rotate, double smoothing, bool followHeight, int[] rotateOffset, int[] positionOffset)
        {
            dynamic packetData = new
            {
                route = routeId,
                node = nodeId,
                speed = speed,
                offset = offset,
                rotate = rotate,
                smoothing = smoothing,
                followHeight = followHeight,
                rotateOffset = rotateOffset,
                positionOffset = positionOffset

            };
            return CommandUtils.Wrap(packetData, prefix + "follow");
        }

        /**
         * This method changes the speed of a nod on a route
         */
        public static dynamic Speed(string nodeId, double speed)
        {
            dynamic packetData = new
            {
                node = nodeId,
                speed = speed
            };
            return CommandUtils.Wrap(packetData, prefix + "follow/speed");
        }

        /**
         * This method shows the route with red lining.
         */
        public static dynamic ShowRoute(bool show)
        {
            dynamic packetData = new
            {
                show = show
            };
            return CommandUtils.Wrap(packetData, prefix + "show");
        }
    }
}
