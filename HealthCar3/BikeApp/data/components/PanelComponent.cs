namespace BikeApp.data.components
{
    /*
     * This class is a panel component for the VR environment.
     */
    internal class PanelComponent
    {
        public double[] size;
        public int[] resolution;
        public int[] background;
        public bool castShadow;

        /*
         * Initializes the values.
         */
        public PanelComponent(double height, double width, int resX, int resY, int bg1, int bg2, int bg3, int bg4, bool hasCastShadow)
        {
            size = new double[] { height, width };
            resolution = new int[] { resX, resY };
            background = new int[] { bg1, bg2, bg3, bg4 };
            castShadow = hasCastShadow;
        }
    }
}
