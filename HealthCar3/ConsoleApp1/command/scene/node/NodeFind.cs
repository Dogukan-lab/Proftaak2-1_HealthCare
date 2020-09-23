using ConsoleApp1.data;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp1.command.scene.node
{
    class NodeFind : VpnCommand<NodeData>
    {
        public NodeFind() : base(id : "scene/node/find")
        {
        }
    }
}
