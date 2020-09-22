using ConsoleApp1.data;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp1
{
    class DunnyTunnel : VpnCommand
    {
        public DunnyTunnel() : base(id: "tunnel/send")
        {
            ConnectData data = new ConnectData();
            data.SetDestination("halloikbeneenbestemming!!!!");
            this.data = data;
        }
    }
}
