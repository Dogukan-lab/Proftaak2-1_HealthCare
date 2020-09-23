using ConsoleApp1.data;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp1.command.scene.node
{
    class NodeMoveTo : VpnCommand<NodeData>
    {
        public NodeMoveTo() : base(id : "scene/node/moveto")
        {
        }
    }
}
