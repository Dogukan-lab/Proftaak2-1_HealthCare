using System;
using System.IO;
using System.Net.Sockets;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Server
{

    class Client
    {
        private TcpClient tcpClient;
        private NetworkStream stream;
        private byte[] buffer = new byte[1024];
        private string totalBuffer = "";
        private List<ClientData> data;
        private List<string> testData;


        public Client(TcpClient tcpClient)
        {
            this.tcpClient = tcpClient;
            this.stream = this.tcpClient.GetStream();
            this.data = new List<ClientData>();
            this.testData = new List<dynamic>();
      
            stream.BeginRead(buffer, 0, buffer.Length, new AsyncCallback(OnRead), null);

        }
        /*
         * Method that deserilises the JsonData
         */
        private void OnRead(IAsyncResult ar)
        {
            try
            {
                int receivedBytes = stream.EndRead(ar);
                string receivedText = System.Text.Encoding.ASCII.GetString(buffer, 0, receivedBytes);
                /*totalBuffer += receivedText;*/
                string a = JsonConvert.DeserializeObject(totalBuffer);
                testData.Add(a);

                Console.WriteLine(testData);
            }
            catch (IOException)
            {
                /*Program.Disconnect(this);
                return;*/
            }

            //TODO 
            //Read and print out the data from the message.
            //Have to save data from client on server.

            stream.BeginRead(buffer, 0, buffer.Length, new AsyncCallback(OnRead), null);
        }




    }
}