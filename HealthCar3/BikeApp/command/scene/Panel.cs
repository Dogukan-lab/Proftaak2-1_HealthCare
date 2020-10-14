using System;
using System.Collections.Generic;
using System.Text;

namespace BikeApp.command.scene
{
    /**
     * This class is used to create a panel
     * TODO Utils class for the wrapper.
     */ 
    static class Panel
    {
        static string prefix = "scene/panel/";

        /**
         * Clears the desired panel
         */
        public static dynamic Clear(string id)
        {
            dynamic packetData = new
            {
                id = id
            };
            return CommandUtils.Wrap(packetData, prefix + "clear");                          
        }

        /**
         * This method swaps the buffers for the desired panel
         */
        public static dynamic Swap(string id)
        {
            dynamic packetData = new
            {
                id = id
            };
            return CommandUtils.Wrap(packetData, prefix + "swap");
        }

        /**
         * This method draws multiple lines on the the backbuffer for the desired panel
         */
        public static dynamic DrawLines(string id, int width, int[,] lines)
        {
            dynamic packetData = new
            {
                id = id,
                widht = width,
                lines = lines
            };
            return CommandUtils.Wrap(packetData, prefix + "drawlines");
        }

        /**
         * This method changes the clear color on the desired panel
         */
        public static dynamic SetClearColor(string id, int[] color)
        {
            dynamic packetData = new
            {
                id = id,
                color = color
            };
            return CommandUtils.Wrap(packetData, prefix + "setclearcolor");
        }

        /**
         * This method draws a text on the desired panel
         */
        public static dynamic DrawText(string id, string text, double[] position, double size, int[] color, string font)
        {
            dynamic packetData = new
            {
                id = id,
                text = text,
                position = position,
                size = size,
                color = color,
                font = font
            };
            return CommandUtils.Wrap(packetData, prefix + "drawtext");
        }

        /**
         * This method draws an image on the desired panel
         */
        public static dynamic Image(string id, string image, double[] position, double[] size)
        {
            dynamic packetData = new
            {
                id = id,
                image = image,
                position = position,
                size = size

            };
            return CommandUtils.Wrap(packetData, prefix + "image");
        }

        
    }
}
