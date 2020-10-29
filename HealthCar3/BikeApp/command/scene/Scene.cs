namespace BikeApp.command.scene
{
    public static class Scene
    {
        /*
         * A prefix to not repeat the same string over and over.
         */
        private const string Prefix = "scene/";

        /*
         * This method gets the data inside of the scene in json format
         */
        public static dynamic Get()
        {
            dynamic packetData = new { };
            return CommandUtils.Wrap(packetData, Prefix + "get");
        }

        /*
         * This method resets the scene to it's default state
         */
        public static dynamic Reset()
        {
            dynamic packetData = new { };
            return CommandUtils.Wrap(packetData, Prefix + "reset");
        }

        /*
         * This method saves the scene in a designated file.
         */
        public static dynamic Save(string filename)
        {
            dynamic packetData = new
            {
                filename,
                overwrite = true
            };
            return CommandUtils.Wrap(packetData, Prefix + "save");
        }

        /*
         * This method loads the scene from the save file
         */
        public static dynamic Load(string filename)
        {
            dynamic packetData = new {filename};
            return CommandUtils.Wrap(packetData, Prefix + "load");
        }

        /*
         * This method casts a ray through the scene, and returns an array of collision points.
         */
        public static dynamic Raycast(int[] start, int[] direction, bool physics)
        {
            dynamic packetData = new
            {
                start,
                direction,
                physics
            };
            return CommandUtils.Wrap(packetData, Prefix + "raycast");
        }
    }
}