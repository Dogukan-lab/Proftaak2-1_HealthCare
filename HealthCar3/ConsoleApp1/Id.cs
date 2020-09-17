using System;
using Newtonsoft.Json;

namespace ConsoleApp1
{
    class Id : Payload
    {
        public string id;

        public Id(string id)
        {
            this.id = id;
        }
    }
}
