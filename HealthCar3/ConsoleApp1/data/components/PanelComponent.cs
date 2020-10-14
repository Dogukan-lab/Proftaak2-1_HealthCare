using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp1.data.components
{
    class PanelComponent
    {
        public double[] size;
        public int[] resolution;
        public int[] background;
        public bool castShadow;

        public PanelComponent(double height, double width, int resx, int resy, int bg1, int bg2, int bg3, int bg4, bool castShadow)
        {
            this.size = new double[] { height, width };
            this.resolution = new int[] { resx, resy };
            this.background = new int[] { bg1, bg2, bg3, bg4 };
            this.castShadow = castShadow;
        }
    }
}
