using ConsoleApp1.data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace ConsoleApp1
{
    class DunnyTunnel : VpnCommand<ConnectData>
    {
        public DunnyTunnel(string dest) : base(id: "tunnel/send")
        {
            GetData().SetDestination(dest);
        }
    }
}
