using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp1.command.scene
{
    class Terrain
    {
        int[] size;
        int[] height;

        public Terrain(int[] size)
        {
            this.size = size;
            this.height = new int[65536];
        }

        public dynamic AddTerrain(int[] size, int[] height)
        {
            dynamic packetData = new
            {
                size = size,
                height = height
            };
            return packetData;
        }

        
    }
}
