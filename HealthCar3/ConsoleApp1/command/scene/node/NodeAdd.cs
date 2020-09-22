using ConsoleApp1.data;

namespace ConsoleApp1.command.scene.node
{
    class NodeAdd : VpnCommand
    {
        public NodeAdd(NodeData data) : base(id: "scene/node/add", data: data)
        {
        }
    }
}
