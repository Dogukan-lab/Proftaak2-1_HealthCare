using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp1.data.components
{
    class TransformComponent
    {
        public int[] position;
        public int scale;
        public int[] rotation;

        public TransformComponent(int posx, int posy, int posz, int scale, int rotx, int roty, int rotz)
        {
            this.position = new int[] { posx, posy, posz };
            this.scale = scale;
            this.rotation = new int[] { rotx, roty, rotz };
        }
    }
}
