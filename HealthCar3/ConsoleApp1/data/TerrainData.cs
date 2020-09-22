using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp1.data
{
    class TerrainData : VpnData
    {
        public int[] size;
        public int[] height;

        public int[] GetSize()
        {
            return this.size;
        }

        public int[] GetHeight()
        {
            return this.height;
        }

        public void SetHeight(int val)
        {
            this.height = new int[] { val };
        }

        public void SetSize(int width, int height)
        {
            this.size = new int[] { width, height };
        }
    }
}
