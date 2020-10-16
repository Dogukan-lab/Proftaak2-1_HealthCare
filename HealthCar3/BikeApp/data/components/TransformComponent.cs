namespace BikeApp.data.components
{
    /*
     * A transform component to be used inside of the VR environment.
     */
    internal class TransformComponent
    {
        public double[] position;
        public double scale;
        public double[] rotation;

        /*
         * Initializes the values.
         */
        public TransformComponent(double posX, double posY, double posZ, double scaling, double rotX, double rotY, double rotZ)
        {
            position = new [] { posX, posY, posZ };
            scale = scaling;
            rotation = new [] { rotX, rotY, rotZ };
        }
        
        public TransformComponent(dynamic pos, double scaling, dynamic rot)
        {
            position = pos;
            scale = scaling;
            rotation = rot;
        }
    }
}
