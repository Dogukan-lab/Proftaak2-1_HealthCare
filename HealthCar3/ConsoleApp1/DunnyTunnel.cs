using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp1
{
    class DunnyTunnel : VpnCommand
    {
        public DunnyTunnel() : base(id: "tunnel/send")
        {
            this.data = new VpnData();
            this.data.SetDestination("halloikbeneenbestemming!!!!");
        }
    }
}
