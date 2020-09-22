using ConsoleApp1.data;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp1.command.scene.terrain
{
    class TerrainAdd : VpnCommand
    {
        public TerrainAdd() : base(id : "scene/terrain/add")
        {
            TerrainData data = new TerrainData();
            data.SetSize(256, 256);
            data.SetHeight(0);
            this.data = data;
        }
    }
}
