using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp1.data.components
{
    class ModelComponent
    {
        public string file;
        public bool cullbackfaces;
        public bool animated;
        public string animation;

        public ModelComponent(string file, bool cullbackfaces, bool animated, string animation)
        {
            this.file = file;
            this.cullbackfaces = cullbackfaces;
            this.animated = animated;
            this.animation = animation;
        }
    }
}
