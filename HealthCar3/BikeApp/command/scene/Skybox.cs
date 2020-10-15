namespace BikeApp.command.scene
{
    internal static class Skybox
    {
        /*
         * A prefix to not repeat the same string over and over.
         */
        private const string Prefix = "scene/skybox/";

        /*
         * This method sets the time inside of the vr environment
         */
        public static dynamic SetTime(int time)
        {
            dynamic packetData = new { time = time };
            return CommandUtils.Wrap(packetData, Prefix + "settime");
        }

        /*
         * This method updates or changes the skybox. For instance update the time to change the skybox to night.
         */
        public static dynamic Update(string skyboxType, string xPos, string xNeg, string yPos, string yNeg, string zPos, string zNeg)
        {
            dynamic packetData = new
            {
                type = skyboxType,
                files = new
                {
                    xpos = xPos,
                    xneg = xNeg,
                    ypos = yPos,
                    yneg = yNeg,
                    zpos = zPos,
                    zneg = zNeg
                }
            };
            return CommandUtils.Wrap(packetData, Prefix + "update");
        }
    }
}
