using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp1.command.scene
{
    static class Skybox
    {
        static string prefix = "scene/skybox/";

        /**
         * This method sets the time inside of the vr environment
         */
        public static dynamic SetTime(double time)
        {
            dynamic packetData = new
            {
                time = time
            };
            return CommandUtils.Wrap(packetData, prefix + "settime");
        }

        /**
         * This method updates or changes the skybox. For instance update the time to change the skybox to night.
         */
        public static dynamic Update(string type, string xPos, string xNeg, string yPos, string yNeg, string zPos, string zNeg )
        {
            dynamic packetData = new
            {
                type = type,
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
            return CommandUtils.Wrap(packetData, prefix + "update");
        }

    }
}
