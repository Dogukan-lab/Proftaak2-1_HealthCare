using ConsoleApp1.data;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp1.command.scene.node
{
    class NodeUpdate : VpnCommand<NodeData>
    {
        public NodeUpdate() : base(id : "scene/node/update")
        {
        }
    }
}
