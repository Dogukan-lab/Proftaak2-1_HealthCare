using ConsoleApp1.data;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp1.command.scene.node
{
    class NodeDellayer : VpnCommand<NodeData>
    {
        public NodeDellayer() : base(id: "scene/node/dellayer")
        {
        }
    }
}
