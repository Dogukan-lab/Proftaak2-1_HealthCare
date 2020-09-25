using ConsoleApp1.command.scene;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp1.command.route
{
    static class Route
    {

        static string prefix = "scene/road/";

        /**
         * This method adds a route to the vr system
         */
        public static dynamic AddRoute( object[] nodes)
        {
            dynamic packetData = new
            {
                nodes = nodes
            };
            return SceneUtils.Wrap(packetData, prefix + "add");
        }

        public static dynamic UpdateRoute(string uuid, object[] nodes)
        {
            dynamic packetData = new
            {
                uuid = uuid,
                nodes = nodes
            };
            return SceneUtils.Wrap(packetData, prefix + "update");
        }

        public static dynamic DeleteRoute(string uuid)
        {
            dynamic packetData = new
            {
                uuid = uuid
            };
            return SceneUtils.Wrap(packetData, prefix + "delete");
        }

        public static dynamic FollowRoute(string routeId, string nodeId, double speed, double offset, string rotate, double smoothing, bool followHeight, int[] rotateOffset, int[] positionOffset)
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
            return SceneUtils.Wrap(packetData, prefix + "follow");
        }

        public static dynamic SpeedRoute(string nodeId, double speed)
        {
            dynamic packetData = new
            {
                node = nodeId,
                speed = speed
            };
            return SceneUtils.Wrap(packetData, prefix + "speed");
        }

        public static dynamic ShowRoute(bool show)
        {
            dynamic packetData = new
            {
                show = show
            };
            return SceneUtils.Wrap(packetData, prefix + "show");
        }
    }
}
