using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp1.command.scene
{
    static class CommandUtils
    {
        private static int serial;
        public static int GetSerial() { return serial; }
        public static void SetSerial(int newSerial) { serial = newSerial; }
        /**
         * This method wraps the data into the send message.
         */
        public static dynamic Wrap(dynamic data, string id)
        {
            dynamic packet = new
            {
                id = id,
                serial = serial,
                data = data
            };
            return packet;
        }
    }
}
