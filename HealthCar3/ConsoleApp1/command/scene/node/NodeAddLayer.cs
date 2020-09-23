using ConsoleApp1.data;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp1.command.scene.node
{
    class NodeAddlayer : VpnCommand<NodeData>
    {
        public NodeAddlayer() : base(id: "scene/node/addlayer")
        {
        }
    }
}
