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

        public MessageParser(VpnConnector connector)
        {
            this.connector = connector;
        }

        /*
         * This method checks the current session for your computer name.
         * Then it gets the id en sets it inside of the for loop.
         */
        public void GetSession(String data)
        {    
            dynamic jsonData = JsonConvert.DeserializeObject(data);
            for (int i = 0; i < jsonData.data.Count; i++)
            {
                if (jsonData.data[i].clientinfo.host == Environment.MachineName)
                {
                    this.id = (string)jsonData.data[i].id;
                    Console.WriteLine(id);
                }
            }
            
        }
        
        /**
         * Parses the data received based on the given response id.
         */
        public void Parse(string id, String data)
        {
            switch (id)
            {
                case "session/list":
                    GetSession(data);
                    break;
            }
        }
            
    }
}
