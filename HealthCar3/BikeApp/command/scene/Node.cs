using BikeApp.data.components;

namespace BikeApp.command.scene
{
    /*
     *  This class creates, finds and deletes nodes.
     */
    internal static class Node
    {
        /*
         * A prefix to not repeat the same string over and over.
         */
        private const string Prefix = "scene/node/";

        /*
         * This method adds a model such a tree, bike or house.
         */
        public static dynamic AddModel(string objectName, TransformComponent transformComponent, ModelComponent modelComponent)    
        {
            dynamic packetData = new
            {
                name = objectName,
                components = new
                {
                    transform = transformComponent,
                    model = modelComponent
                }
            };
            return CommandUtils.Wrap(packetData, Prefix + "add");
        }

        /*
         * This method adds a terrain node to be used for texturing.
         */
        public static dynamic AddTerrain(string terrainName, string parentName, TransformComponent transformComponent, bool smoothnormals)
        {
            dynamic packetData = new
            {
                name = terrainName,
                parent = parentName,
                components = new
                {
                    transform = transformComponent,
                    terrain = new
                    {
                        smoothnormals = smoothnormals
                    }
                }
            };
            return CommandUtils.Wrap(packetData, Prefix + "add");
        }

        /*
         * This method adds a panel as a node.
         */
        public static dynamic AddPanel(string panelName, string parentName, PanelComponent panelComponent, TransformComponent transformComponent)
        {
            dynamic packetData = new
            {
                name = panelName,
                parent = parentName,
                components = new
                {
                    panel = panelComponent,
                    transform = transformComponent
                }
            };
            return CommandUtils.Wrap(packetData, Prefix + "add");
        }

        /*
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
            return CommandUtils.Wrap(packetData, Prefix + "addlayer");
        }

        /*
         * This method can delete a layer.
         */
        public static dynamic DelLayer()
        {
            dynamic packetData = new {};
            return CommandUtils.Wrap(packetData, Prefix + "dellayer");
        }

        /*
         * This method can find a specified node to be edited or used.
         */
        public static dynamic Find(string objectName)
        {
            dynamic packetData = new { name = objectName };
            return CommandUtils.Wrap(packetData, Prefix + "find");
        }

        /*
         * This method can move a node.
         */
        public static dynamic MoveTo(string uuid, string stop, int[] currentPosition, string rotation, string interpolation, bool followheight, double setSpeed)
        {
            dynamic packetData = new
            {
                id = uuid,
                stop = stop,
                position = currentPosition,
                rotate = rotation,
                interpolate = interpolation,
                followheight = followheight,
                speed = setSpeed
            };
            return CommandUtils.Wrap(packetData, Prefix + "moveto");
        }

        /*
         * This method updates the position of a node.
         */
        public static dynamic Update(string guid, string parentObject, TransformComponent transformComponent, string animationName, double setSpeed)
        {
            dynamic packetData = new
            {
                id = guid,
                parent = parentObject,
                transform = transformComponent,
                animation = new
                {
                    name = animationName,
                    speed = setSpeed
                }
            };
            return CommandUtils.Wrap(packetData, Prefix + "update");
        }

        /*
         * This method deletes a specified node.
         */
        public static dynamic Delete(string guid)
        {
            dynamic packetData = new { id = guid };
            return CommandUtils.Wrap(packetData, Prefix + "delete");
        }
    }
}
