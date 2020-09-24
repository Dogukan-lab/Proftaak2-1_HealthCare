using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp1.command.scene
{
    static class Panel
    {
        static string prefix = "scene/panel/";

        private static dynamic Wrap(dynamic panelData, string id)
        {
            dynamic packet = new
            {
                id = id,
                data = panelData
            };
            return packet;
        }

        public static dynamic Clear(string id)
        {
            dynamic packetData = new
            {
                id = id
            };
            return Wrap(packetData, prefix + "clear");                          
        }

        public static dynamic Swap(string id)
        {
            dynamic packetData = new
            {
                id = id
            };
            return Wrap(packetData, prefix + "swap");
        }

        public static dynamic DrawLines(string id, int width, int[,] lines)
        {
            dynamic packetData = new
            {
                id = id,
                widht = width,
                lines = lines
            };
            return Wrap(packetData, prefix + "drawlines");
        }

        public static dynamic SetClearColor(string id, int[] color)
        {
            dynamic packetData = new
            {
                id = id,
                color = color
            };
            return Wrap(packetData, prefix + "setclearcolor");
        }

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
            return Wrap(packetData, prefix + "drawtext");
        }

        public static dynamic Image(string id, string image, double[] position, double[] size)
        {
            dynamic packetData = new
            {
                id = id,
                image = image,
                position = position,
                size = size

            };
            return Wrap(packetData, prefix + "image");
        }

        
    }
}
