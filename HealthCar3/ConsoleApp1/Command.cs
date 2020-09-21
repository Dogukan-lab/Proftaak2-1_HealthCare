using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp1
{
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
