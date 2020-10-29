namespace BikeApp.command.scene
{
    /*
     * This class is used to create a panel
     */ 
    internal static class Panel
    {
        /*
         * A prefix to not repeat the same string over and over.
         */
        private const string Prefix = "scene/panel/";

        /*
         * Clears the current panel
         */
        public static dynamic Clear(string panelUuid)
        {
            dynamic packetData = new { id = panelUuid };
            return CommandUtils.Wrap(packetData, Prefix + "clear");                          
        }

        /*
         * This method swaps the buffers for the current panel
         */
        public static dynamic Swap(string panelUuid)
        {
            dynamic packetData = new { id = panelUuid };
            return CommandUtils.Wrap(packetData, Prefix + "swap");
        }

        /*
         * This method draws multiple lines on the the backbuffer for the desired panel
         */
        public static dynamic DrawLines(string panelUuid, int width, int[,] lines)
        {
            dynamic packetData = new
            {
                id = panelUuid,
                width = width,
                lines = lines
            };
            return CommandUtils.Wrap(packetData, Prefix + "drawlines");
        }

        /*
         * This method changes the clear color on the desired panel
         */
        public static dynamic SetClearColor(string panelUuid, int[] color)
        {
            dynamic packetData = new
            {
                id = panelUuid,
                color = color
            };
            return CommandUtils.Wrap(packetData, Prefix + "setclearcolor");
        }

        /*
         * This method draws a text on the current panel
         */
        public static dynamic DrawText(string panelUuid, string text, int[] position, double size, int[] color, string font)
        {
            dynamic packetData = new
            {
                id = panelUuid,
                text = text,
                position = position,
                size = size,
                color = color,
                font = font
            };
            return CommandUtils.Wrap(packetData, Prefix + "drawtext");
        }

        /*
         * This method draws an image on the current panel
         */
        public static dynamic Image(string panelUuid, string desiredImage, double[] position, double[] size)
        {
            dynamic packetData = new
            {
                id = panelUuid,
                image = desiredImage,
                position = position,
                size = size
            };
            return CommandUtils.Wrap(packetData, Prefix + "image");
        }
    }
}
