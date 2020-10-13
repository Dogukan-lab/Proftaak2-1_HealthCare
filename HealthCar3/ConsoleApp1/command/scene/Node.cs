﻿using ConsoleApp1.data.components;
using Newtonsoft.Json.Linq;
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
         * This method adds a model such a tree, bike or house.
         */
        public static dynamic AddModel(string objectName, TransformComponent transform, ModelComponent model)
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
            return CommandUtils.Wrap(packetData, prefix + "add");
        }

        /**
         * This method adds a terrain node to be used for texturing.
         */
        public static dynamic AddTerrain(string name, string parent, TransformComponent transform, bool smoothnormals)
        {
            dynamic packetData = new
            {
                name = name,
                parent = parent,
                components = new
                {
                    transform = transform,
                    terrain = new
                    {
                        smoothnormals = smoothnormals
                    }
                }
            };
            return CommandUtils.Wrap(packetData, prefix + "add");
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
            return CommandUtils.Wrap(packetData, prefix + "add");
        }

        /**
         * This method can add a layer to the vr system
         */
        public static dynamic AddLayer(string uuid, string diffusePng, string normalPng, int minHeight, int maxHeight, double fadeDist)
        {
            dynamic packetData = new
            {
                id = uuid,
                diffuse = diffusePng,
                normal = normalPng,
                minHeight = minHeight,
                maxHeight = maxHeight,
                fadeDist = fadeDist
            };
            return CommandUtils.Wrap(packetData, prefix + "addlayer");
        }

        /**
         * This method can delete a layer.
         */
        public static dynamic DelLayer()
        {
            dynamic packetData = new
            {

            };
            return CommandUtils.Wrap(packetData, prefix + "dellayer");
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
            return CommandUtils.Wrap(packetData, prefix + "find");
        }

        /**
         * This method can move a node.
         */
        public static dynamic MoveTo(string guid, string stop, int[] position, string rotate, string interpolate, bool followheight, double speed)
        {
            dynamic packetData = new
            {

                id = guid,
                stop = stop,
                position = position,
                rotate = rotate,
                interpolate = interpolate,
                followheight = followheight,
                speed = speed
            };
            return CommandUtils.Wrap(packetData, prefix + "moveto");
        }

        /**
         * This method updates the position of a node.
         */
        public static dynamic Update(string guid, string parent, TransformComponent transform, string name, double speed)
        {
            dynamic packetData = new
            {
                id = guid,
                parent = parent,
                transform = transform,
                animation = new
                {
                    name = name,
                    speed = speed
                }
            };
            return CommandUtils.Wrap(packetData, prefix + "update");
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
            return CommandUtils.Wrap(packetData, prefix + "delete");

        }
    }
}
