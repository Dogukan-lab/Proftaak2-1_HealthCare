using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp1.command.scene
{
    static class Skybox
    {
        static string prefix = "scene/skybox/";

        private static dynamic Wrap(dynamic skyboxData, string id)
        {
            dynamic packet = new
            {
                id = id,
                data = skyboxData
            };
            return packet;
        }

        public static dynamic SetTime(int time)
        {
            dynamic packetData = new
            {
                time = time
            };
            return Wrap(packetData, prefix + "settime");
        }

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
            return Wrap(packetData, prefix + "update");
        }

    }
}
