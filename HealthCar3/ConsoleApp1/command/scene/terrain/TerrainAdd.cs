using ConsoleApp1.data;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp1.command.scene.terrain
{
    /**
     * This Class is to add the terrain into the network application
     */
    class TerrainAdd : VpnCommand
    {
        public TerrainAdd(TerrainData data) : base(id : "scene/terrain/add", data : data)
        {


        }
    }
}
