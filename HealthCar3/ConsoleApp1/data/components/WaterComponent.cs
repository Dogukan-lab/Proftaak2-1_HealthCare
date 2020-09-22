using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp1.data.components
{
    class WaterComponent
    {
        public int[] size;
        public double resolution;

        public WaterComponent(int x, int y, double resolution)
        {
            this.size = new int[] { x, y };
            this.resolution = resolution;
        }
    }
}
