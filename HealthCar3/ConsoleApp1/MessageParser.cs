using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp1
{
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
