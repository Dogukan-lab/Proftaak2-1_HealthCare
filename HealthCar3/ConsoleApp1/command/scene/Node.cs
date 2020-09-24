using ConsoleApp1.data.components;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace ConsoleApp1.command.scene
{
    /**
     *  This class creates, finds and deletes nodes.
     */
    static class Node
    {
        private static string prefix = "scene/node/";

     
        /**
         * This wrap method wraps the node data into a command.
         */
        private static dynamic wrap(dynamic nodeData, string id)
        {
            dynamic packet = new
            {
                id = id,
                data = nodeData
            };
            return packet;
        }

        /**
         * This method adds a model such a tree, bike or house.
         */
        public static dynamic AddModel(string objectName, string objectFile, TransformComponent transform, ModelComponent model)
        {
            dynamic packetData = new
            {
                name = objectName,
                components = new
                {
                    transform = transform,
                    model = model
                }
            };
            return wrap(packetData, prefix + "add");
        }

        /**
         * This method adds a terrain node to be used for texturing.
         */
        public static dynamic AddTerrain(string name, string parent, ModelComponent model, Boolean smoothnormals )
        {
            dynamic packetData = new
            {
                name = name,
                parent = parent,
                components = new
                {
                    model = model,
                    terrain = new
                    {
                        smoothnormals = smoothnormals
                    }
                }
            };
            return wrap(packetData, prefix + "add");
        }

        /**
         * This method adds a panel as a node.
         */
        public static dynamic AddPanel(string name, string parent, PanelComponent panel)
        {
            dynamic packetData = new
            {
                name = name,
                parent = parent,
                components = new
                {
                    panel = panel
                }
            };
            return wrap(packetData, prefix + "add");
        }

        /**
         * This method can add a layer to the vr system
         */
        public static dynamic AddLayer(string id, object diffuse, object normal, int minHeight, int maxHeight, int fadeDist)
        {
            dynamic packetData = new
            {
                id = id,
                diffuse = diffuse,
                normal = normal,
                minHeight = minHeight,
                maxHeight = maxHeight,
                fadeDist = fadeDist
            };
            return wrap(packetData, prefix + "add");
        }

        /**
         * This method can delete a layer.
         */
        public static dynamic DelLayer()
        {
            dynamic packetData = new
            {

            };
            return wrap(packetData, prefix + "dellayer");
        }

        /**
         * This method can find a specified node to be edited or used.
         */
        public static dynamic Find(string objectName)
        {
            dynamic packetData = new
            {
                name = objectName
            };
            return wrap(packetData, prefix + "find");
        }

        /**
         * This method can move a node.
         */
        public static dynamic MoveTo(string id, string stop, int[] position, string rotate, string interpolate, Boolean followheight, double speed)
        {
            dynamic packetData = new
            {

                id = id,
                stop = stop,
                position = position,
                rotate = rotate,
                interpolate = interpolate,
                followheight = followheight,
                speed = speed
            };
            return wrap(packetData, prefix + "moveto");
        }

        /**
         * This method updates the position of a node.
         */
        public static dynamic Update(string id, object parent, TransformComponent transform, string name, double speed)
        {
            dynamic packetData = new
            {
                id = id,
                parent = parent,
                transform = transform,
                animation = new
                {
                    name = name,
                    speed = speed
                }
            };
            return wrap(packetData, prefix + "update");
        }

        /**
         * This method deletes a specified node.
         */
        public static dynamic Delete(string guid)
        {
            dynamic packetData = new
            {
                id = guid
            };
            return wrap(packetData, prefix + "delete");

        }
    }
}
