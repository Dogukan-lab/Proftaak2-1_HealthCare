using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp1
{
    //(Not finished) Class that is supposed to be used to send commands to the server.
    class Command
    {
        private String id;
        private VpnConnector vpnConnector;
        private VpnData tempData;
        private List<String> data;

        public Command(String id, List<String> data)
        {
            this.id = id;
            this.data = data;
        }

        public void CreateCommand(String data, String id)
        {
            tempData = new VpnData(data);
            VpnCommand command = new VpnCommand(id, tempData);
            vpnConnector.Send(command);
        }

        public void DeleteNodeObject()
        {
            CreateCommand("{guid}", "scene/node/delete");
        }

        public void DeleteTerrainObject()
        {
            CreateCommand(null, "scene/terrain/delete");
        }

        public void DeleteRouteObject()
        {
            CreateCommand("uuid", "route/delete");
        }

    }
}
