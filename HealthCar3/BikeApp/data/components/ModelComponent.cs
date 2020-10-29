namespace BikeApp.data.components
{
    /*
     * This is a component for models inside of the VR environment.
     */
    public class ModelComponent
    {
        public bool animated;
        public string animation;
        public bool cullbackfaces;
        public string file;

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