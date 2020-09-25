using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp1.command.scene
{
    static class CommandUtils
    {
        /**
         * This method wraps the data into the send message.
         */
        public static dynamic Wrap(dynamic data, string id)
        {
            dynamic packet = new
            {
                id = id,
                data = data
            };
            return packet;
        }
    }
}
