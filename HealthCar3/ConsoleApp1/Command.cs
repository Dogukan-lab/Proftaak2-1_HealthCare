using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp1
{
    //(Not finished) Class that is supposed to be used to send commands to the server.
    class Command
    {
        private String id;

        private List<String> data;

        public Command(String id, List<String> data)
        {
            this.id = id;
            this.data = data;
        }
    }
}
