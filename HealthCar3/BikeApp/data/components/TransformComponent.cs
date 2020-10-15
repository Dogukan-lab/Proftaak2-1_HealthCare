namespace BikeApp.data.components
{
    /*
     * A transform component to be used inside of the VR environment.
     */
    internal class TransformComponent
    {
        public double[] position;
        public double scale;
        public int[] rotation;

        /*
         * Initializes the values.
         */
        public TransformComponent(double posX, double posY, double posZ, double scaling, int rotX, int rotY, int rotZ)
        {
            position = new double[] { posX, posY, posZ };
            scale = scaling;
            rotation = new int[] { rotX, rotY, rotZ };
        }
    }
}
