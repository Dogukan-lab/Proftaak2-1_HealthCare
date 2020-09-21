using System;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace ConsoleApp1
{
    class Data : IPayload
    {
        public string id;
        public List<String> data;

        public Data(string id, String session, String key)         {
            this.id = id;
            data = new List<string>();
            data.Add(session);
            data.Add(key);
        }

     /*   public Data(String id, String dest, Command data)
        {
            this.id = id;
            this.dest = dest;

        }*/

    }
}
