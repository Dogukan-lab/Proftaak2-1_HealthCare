using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Text;

namespace ConsoleApp1.data.components
{
    class ComponentMashup
    {
        public TransformComponent transform;
        public ModelComponent model;
        public TerrainComponent terrain;
        public PanelComponent panel;
        public WaterComponent water;

        public ComponentMashup(TransformComponent transform, ModelComponent model, TerrainComponent terrain, PanelComponent panel, WaterComponent water)
        {
            this.transform = transform;
            this.model = model;
            this.terrain = terrain;
            this.panel = panel;
            this.water = water;
        }
    }
}
