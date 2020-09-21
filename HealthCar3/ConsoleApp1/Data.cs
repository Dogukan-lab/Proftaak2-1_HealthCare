using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Transactions;

namespace ConsoleApp1
{
    class Data : VpnCommand
    {
        public List<String> data;

        public Data(string id, String session, String key) : base(id)        {
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
