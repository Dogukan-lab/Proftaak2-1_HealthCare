using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp1
{
    class DunnyCommand : VpnCommand
    {
        public DunnyCommand() : base(id: "node/create")
        {
            this.data = generateData();
        }

        private DunnyData generateData()
        {
            return new DunnyData("test");
        }
    }
}
