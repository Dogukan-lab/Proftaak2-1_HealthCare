using ConsoleApp1.data;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp1.command.scene.terrain
{
    class TerrainDelete : VpnCommand<TerrainData>
    {
        public TerrainDelete() : base(id : "scene/terrain/delete")
        {
            
        }
    }
}
