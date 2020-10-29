namespace BikeApp.command.scene
{
    public static class Terrain
    {
        /*
         * A prefix to not repeat the same string over and over.
         */
        private const string Prefix = "scene/terrain/";

        /*
         * This method adds a skeleton terrain for the terrain textures.
         */
        public static dynamic Add(int[] size, double[] heightMap)
        {
            dynamic packetData = new
            {
                size,
                heights = heightMap
            };
            return CommandUtils.Wrap(packetData, Prefix + "add");
        }

        /*
         * This method updates the terrain
         */
        public static dynamic Update()
        {
            dynamic packetData = new { };
            return CommandUtils.Wrap(packetData, Prefix + "update");
        }

        /*
         * This method deletes the terrain
         */
        public static dynamic Delete()
        {
            dynamic packetData = new { };
            return CommandUtils.Wrap(packetData, Prefix + "delete");
        }

        /*
         * This method gets the height map of the terrain
         */
        public static dynamic GetHeight(int[] position, int[,] positions)
        {
            dynamic packetData = new
            {
                position, positions
            };
            return CommandUtils.Wrap(packetData, Prefix + "update");
        }
    }
}