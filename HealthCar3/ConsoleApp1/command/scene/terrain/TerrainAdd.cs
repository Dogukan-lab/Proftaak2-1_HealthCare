using ConsoleApp1.data;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp1.command.scene.terrain
{
    /**
     * This Class is to add the terrain into the network application
     */
    class TerrainAdd : VpnCommand<TerrainData>
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
