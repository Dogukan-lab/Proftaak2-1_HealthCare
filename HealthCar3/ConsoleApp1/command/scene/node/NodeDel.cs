using ConsoleApp1.data;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp1.command.scene.node
{
    /**
     * Deletes a node from the scene.
     */
    class NodeDel : VpnCommand<NodeData>
    {

        public NodeDel() : base(id: "scene/node/delete")
        {
            this.data = new NodeData();
        }

        /**
         * Sets the id of the node to remove.
         */
        public void SetId(string id)
        {
            this.data.id = id;
        }
    }
}
