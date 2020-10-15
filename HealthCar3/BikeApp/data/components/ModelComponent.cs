namespace BikeApp.data.components
{
    /*
     * This is a component for models inside of the VR environment.
     */
    internal class ModelComponent
    {
        public string file;
        public bool cullbackfaces;
        public bool animated;
        public string animation;

        /*
         * Constructor to initialize the values.
         */
        public ModelComponent(string fileName, bool hasCullbackfaces, bool isAnimated, string animationName)
        {
            file = fileName;
            cullbackfaces = hasCullbackfaces;
            animated = isAnimated;
            animation = animationName;
        }
    }
}
