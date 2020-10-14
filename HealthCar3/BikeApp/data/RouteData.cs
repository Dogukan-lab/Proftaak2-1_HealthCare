using System;
using System.Collections.Generic;
using System.Text;

namespace BikeApp.data
{
    class RouteData
    {
        public int[] pos;
        public int[] dir;

        public RouteData(int[] pos, int[] dir)
        {
            this.pos = pos;
            this.dir = dir;
        }
    }
}
