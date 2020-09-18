using System;
using Newtonsoft.Json;

namespace ConsoleApp1
{
    class Id : IPayload
    {
        public string id;

        public Id(string id)
        {
            this.id = id;
        }
    }
}
