namespace BikeApp.data.components
{
    /*
     * A transform component to be used inside of the VR environment.
     */
    internal class TransformComponent
    {
        public int[] position;
        public double scale;
        public int[] rotation;

        /*
         * Initializes the values.
         */
        public TransformComponent(int posX, int posY, int posZ, double scaling, int rotX, int rotY, int rotZ)
        {
            position = new int[] { posX, posY, posZ };
            scale = scaling;
            rotation = new int[] { rotX, rotY, rotZ };
        }
    }
}
