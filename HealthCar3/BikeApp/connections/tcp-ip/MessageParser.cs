using System;
using BikeApp.vr_environment;

namespace BikeApp.connections
{
    /*
     *This class will parse the needed assets to be sent from the connector to the application
     */
    internal class MessageParser
    {
        private readonly VpnConnector vpnConnector;
        private readonly CommandCenter command;
        private string id;
        private string destination;

        public MessageParser(VpnConnector connector, CommandCenter commandCenter)
        {
            vpnConnector = connector;
            command = commandCenter;
        }

        public string GetDestination() { return destination; }

        /*
         * This method checks the current session for your computer name.
         * Then it gets the id and sets it inside of the for loop.
         */
        private void GetSessionId(dynamic jsonData)
        {
            for (var i = 0; i < jsonData.data.Count; i++)
            {
                if (jsonData.data[i].clientinfo.user == Environment.UserName)
                {
                    id = (string)jsonData.data[i].id;
                }
            }
            if (id != null) return;
            Environment.Exit(0);
            throw new Exception(@"Error: Session not found. Please make sure you are connected to the network application!");
        }

        /**
         * Gets the response. From the response it gets
         * the id for the tunnel/send
         */
        private void GetTunnelId(dynamic jsonData)
        {
            if (jsonData.data.status != "ok") return;
            destination = (string)jsonData.data.id;
            Console.WriteLine(@"Destination has been set! {0}", destination);
        }

        /**
         * Parses the data received based on the given response id.
         */
        public void Parse(string uuId, dynamic jsonData)
        {
            switch (uuId)
            {
                case "session/list":
                    try
                    {
                        GetSessionId(jsonData);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    }
                    if (id != null)
                    {
                        Console.WriteLine(@"Creating the tunnel...");
                        // Create the tunnel
                        vpnConnector.Send(new { id = "tunnel/create", data = new { session = id, key = "" } });
                    }
                    break;
                case "tunnel/create":
                    Console.WriteLine(@"Tunnel has been created!");
                    GetTunnelId(jsonData);
                    if (destination != null)
                    {
                        // Scene is now fully initialized and can now execute commands 
                        command.PresetOne();
                    }
                    break;
            }
        }
    }
}
