using System;
using System.Collections.Generic;
using System.Text;

namespace BikeApp.data.components
{
    class TransformComponent
    {
        public double[] position;
        public double scale;
        public int[] rotation;

        public TransformComponent(double posx, double posy, double posz, double scale, int rotx, int roty, int rotz)
        {
            this.position = new double[] { posx, posy, posz };
            this.scale = scale;
            this.rotation = new int[] { rotx, roty, rotz };
        }
    }
}
