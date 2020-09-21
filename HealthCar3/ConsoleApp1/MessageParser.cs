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
        private VpnData tempData;

        public MessageParser(VpnConnector connector)
        {
            this.connector = connector;
        }

        /*
         * This method checks the current session for your computer name.
         * Then it gets the id en sets it inside of the for loop.
         */
        public void GetSession(string data)
        {
            dynamic jsonData = JsonConvert.DeserializeObject(data);
            for (int i = 0; i < jsonData.data.Count; i++)
            {
                if (jsonData.data[i].clientinfo.user == Environment.UserName)
                {
                    this.id = (string)jsonData.data[i].id;
                }
            }
        }

        /**
         * Gets the response. From the response it gets
         * the id for the tunnel/send
         */
        public void GetTunnelId(string data)
        {
            dynamic jsonData = JsonConvert.DeserializeObject(data);
            for (int i = 0; i < jsonData.data.Count; i++)
            {
                if (jsonData.data[0].status == "ok")
                {
                    Console.WriteLine("The status of the response has been reached!");
                }
            }
        }

        /**
         * Parses the data received based on the given response id.
         */
        public void Parse(string id, string data)
        {
            switch (id)
            {
                case "session/list":
                    GetSession(data);
                    tempData = new VpnData("tunnel/create");
                    tempData.SetSession(this.id);
                    tempData.SetKey("");
                    connector.Send(tempData);
                    break;
                case "tunnel/create":
                    GetTunnelId(data);
                    break;
            }
        }


    }
}
