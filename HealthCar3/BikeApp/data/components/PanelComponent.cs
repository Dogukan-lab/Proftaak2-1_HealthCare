namespace BikeApp.data.components
{
    /*
     * This class is a panel component for the VR environment.
     */
    internal class PanelComponent
    {
        public int[] size;
        public int[] resolution;
        public int[] background;
        public bool castShadow;

        /*
         * Initializes the values.
         */
        public PanelComponent(int height, int width, int resx, int resy, int bg1, int bg2, int bg3, int bg4, bool hasCastShadow)
        {
            size = new int[] { height, width };
            resolution = new int[] { resx, resy };
            background = new int[] { bg1, bg2, bg3, bg4 };
            castShadow = hasCastShadow;
        }
    }
}
