using ConsoleApp1.data;

namespace ConsoleApp1.command.scene.node
{
    class NodeAdd : VpnCommand
    {
        /**
         * Generates a command to spawn an object based on the node Data using Components.
         */
        public NodeAdd(NodeData data) : base(id: "scene/node/add", data: data)
        {
        }
    }
}
