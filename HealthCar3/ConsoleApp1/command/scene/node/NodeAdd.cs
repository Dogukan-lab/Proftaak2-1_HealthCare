using ConsoleApp1.data;
using ConsoleApp1.data.components;

namespace ConsoleApp1.command.scene.node
{
    class NodeAdd : VpnCommand<NodeData>
    {
        /**
         * Generates a command to spawn an object based on the node Data using Components.
         */
        public NodeAdd() : base(id: "scene/node/add")
        {
        }

        public void SetName(string name)
        {
            GetData().SetName(name);
        }

        public void SetParent(string parent)
        {
            GetData().SetParent(parent);
        }

        public void SetModelComponent(ModelComponent model)
        {
            GetData().components.SetModelComponent(model);
        }

        public void SetPanelComponent(PanelComponent panel)
        {
            GetData().components.SetPanelComponent(panel);
        }

        public void SetTerrainComponent(TerrainComponent terrain)
        {
            GetData().components.SetTerrainComponent(terrain);
        }

        public void SetTransformComponent(TransformComponent transform)
        {
            GetData().components.SetTransformComponent(transform);
        }

        public void SetWaterComponent(WaterComponent water)
        {
            GetData().components.SetWaterComponent(water);
        }

        public bool Verify()
        {
            if (GetData().GetName() != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
