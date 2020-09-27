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
        private CommandCenter command;
        private string id;
        private string destination;

        public MessageParser(VpnConnector connector)
        {
            this.connector = connector;
            this.command = new CommandCenter(this.connector);
        }

        public string GetDestination() { return this.destination; }

        /*
         * This method checks the current session for your computer name.
         * Then it gets the id and sets it inside of the for loop.
         */
        public void GetSessionId(dynamic jsonData)
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
                Console.WriteLine("Destination has been set! {0}", this.destination);

            }
        }

        /**
         * Parses the data received based on the given response id.
         * TODO first switch-case needs to be refactored. 
         */
        public void Parse(string id, dynamic jsonData)
        {
            switch (id)
            {
                case "session/list":
                    GetSessionId(jsonData);
                    if (this.id != null)
                    {
                        Console.WriteLine("Creating the tunnel...");
                        // Create the tunnel
                        connector.Send(new { id = "tunnel/create", data = new { session = this.id, key = "" } });
                    }
                    break;
                case "tunnel/create":
                    Console.WriteLine("Tunnel has been created!");
                    GetTunnelId(jsonData);
                    if (this.destination != null)
                    {
                        this.command.ResetScene();
                        // Scene is now fully initialized and can now execute commands 

                        //test
                        //connector.SendPacket(new { name = "test1", components = new { transform = new { position = new[] { 1, 1, 1 }, scale = 1, rotation = new[] { 0, 0, 0 } }, model = new { file = "data/NetworkEngine/models/cars/cartoon/Pony_cartoon2.obj", cullbackfaces = true, animated = false } } }, data =>
                        //{
                        //    Console.WriteLine("Added a new node to the scene.");
                        //});
                    }
                    break;
                /*case "tunnel/send":
                    
                    break;*/
            }
        }
    }
}
