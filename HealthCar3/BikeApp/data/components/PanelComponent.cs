using System;
using System.Collections.Generic;
using System.Text;

namespace BikeApp.data.components
{
    class PanelComponent
    {
        public int[] size;
        public int[] resolution;
        public int[] background;
        public bool castShadow;

        public PanelComponent(int height, int width, int resx, int resy, int bg1, int bg2, int bg3, int bg4, bool castShadow)
        {
            this.size = new int[] { height, width };
            this.resolution = new int[] { resx, resy };
            this.background = new int[] { bg1, bg2, bg3, bg4 };
            this.castShadow = castShadow;
        }
    }
}
