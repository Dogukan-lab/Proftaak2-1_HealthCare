using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

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
        public void getSession()
        {    
            dynamic jsonData = JsonConvert.DeserializeObject(connector.JsonData);
            for (int i = 0; i < jsonData.data.Count; i++)
            {
                if (jsonData.data[i].clientinfo.host == Environment.MachineName)
                {
                    this.id = (string)jsonData.data[i].id;
                }
            }
            
        }

        public void Parse(byte[] message)
        {

        }
            
    }
}
