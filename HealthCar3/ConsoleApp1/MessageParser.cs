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
                        ConnectData tempData = new ConnectData();
                        tempData.SetSession(this.id);
                        tempData.SetKey("");
                        VpnCommand command = new VpnCommand("tunnel/create", tempData); //sends a new command including a data object to the connector.
                        connector.Send(command);
                    }
                    break;
                case "tunnel/create":
                    cc = new CommandCenter(GetTunnelId(jsonData));
                    break;
            }
        }
    }
}
