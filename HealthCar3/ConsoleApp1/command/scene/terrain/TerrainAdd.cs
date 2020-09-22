using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp1.command.scene.terrain
{
    class TerrainAdd : VpnCommand
    {
        public TerrainAdd() : base(id : "scene/terrain/add")
        {
            this.data = new VpnData();
            this.data.SetSize(256, 256);
            this.data.SetHeight(0);
        }
    }
}
