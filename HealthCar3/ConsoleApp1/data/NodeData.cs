using ConsoleApp1.data.components;
using System.Collections.Generic;

namespace ConsoleApp1.data
{
    class NodeData : VpnData
    {
        public string name;
        public string parent;
        public ComponentMashup components;

        public NodeData()
        {
        }

        public void SetName(string name)
        {
            this.name = name;
        }

        public void SetParent(string parent)
        {
            this.parent = parent;
        }

        public void SetComponents(ComponentMashup components)
        {
            this.components = components;
        }

        public string GetName()
        {
            return name;
        }

        public string GetParent()
        {
            return parent;
        }
        public ComponentMashup GetComponents()
        {
            return components;
        }
    }
}
