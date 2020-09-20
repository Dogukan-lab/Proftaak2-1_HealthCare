using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp1
{
    /**
     * Parses any messages received by the VpnConnector.
     */
    class MessageParser
    {
        private VpnConnector connector;


        public MessageParser(VpnConnector connector)
        {
            this.connector = connector;
        }

        public void Parse(byte[] message)
        {
            Console.WriteLine(message);
        }
            
    }
}
