namespace BikeApp.data.components
{
    /*
     * This class is a panel component for the VR environment.
     */
    public class PanelComponent
    {
        public int[] background;
        public bool castShadow;
        public int[] resolution;
        public double[] size;

        /*
         * Initializes the values.
         */
        public PanelComponent(double height, double width, int resX, int resY, int bg1, int bg2, int bg3, int bg4,
            bool hasCastShadow)
        {
            size = new[] {height, width};
            resolution = new[] {resX, resY};
            background = new[] {bg1, bg2, bg3, bg4};
            castShadow = hasCastShadow;
        }
    }
}