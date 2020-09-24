using ConsoleApp1.data;
using Newtonsoft.Json;
using System;

namespace ConsoleApp1
{
    /*
     *This class will parse the needed assets to be sent from the connector to the application
     */
    class MessageParser
    {
        private VpnConnector connector;
        private string id;
        private string destination;

        public MessageParser(VpnConnector connector)
        {
            this.connector = connector;
        }

        public string GetDestination() { return this.destination; }

        /*
         * This method checks the current session for your computer name.
         * Then it gets the id en sets it inside of the for loop.
         */
        public void GetSession(dynamic jsonData)
        {
            for (int i = 0; i < jsonData.data.Count; i++)
            {
                if (jsonData.data[i].clientinfo.user == Environment.UserName)
                {
                    this.id = (string)jsonData.data[i].id;
                }
            }
            if (this.id == null) 
            {
                Console.WriteLine("Error: Session not found. Please make sure you are connected to the network application!");
            }
        }

        /**
         * Gets the response. From the response it gets
         * the id for the tunnel/send
         */
        public void GetTunnelId(dynamic jsonData)
        {

            if (jsonData.data.status == "ok")
            {
                this.destination = (string)jsonData.data.id;
            }
        }

        /**
         * Parses the data received based on the given response id.
         * TODO first switch-case needs to be refactored. 
         * This will be based off of the 
         */
        public void Parse(string id, dynamic jsonData)
        {
            CommandCenter cc;

            switch (id)
            {
                case "session/list":
                    GetSession(jsonData);
                    if (this.id != null)
                    {
                        // Create the tunnel
                        connector.Send(new { id = "tunnel/create", data = new { session = this.id, key = "" } });
                    }
                    break;
                case "tunnel/create":
                    GetTunnelId(jsonData);
                    // Scene is now fully initialized and can now execute commands 
                    //cc = new CommandCenter();

                    // test
                    //connector.SendPacket("scene/node/add", new { name = "test", components = new { transform = new { position = new[] { 1, 1, 1 }, scale = 1, rotation = new[] { 0, 0, 0 } } } }, data => {
                    //    Console.WriteLine("Added a new node to the scene.");
                    //    connector.SendPacket("scene/node/add", new { name = "test", components = new { transform = new { position = new[] { 1, 1, 1 }, scale = 1, rotation = new[] { 0, 0, 0 } } } }, data => {
                    //        Console.WriteLine("Added a new node to the scene.");
                    //    });
                    //});
                    break;
            }
        }
    }
}
