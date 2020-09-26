using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp1
{
    class DunnyCommand : VpnCommand<DunnyData>
    {
        public DunnyCommand() : base(id: "node/create")
        {
            GetData().test = "test";
        }
    }
}
