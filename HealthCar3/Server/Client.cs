using System;
using System.IO;
using System.Net.Sockets;
using Newtonsoft.Json;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using System.Text;
using PackageUtils;

namespace Server
{

    class Client
    {
        private TcpClient tcpClient;
        private NetworkStream stream;
        private byte[] buffer = new byte[1024];
        private string id;


        public Client(TcpClient tcpClient)
        {
            this.tcpClient = tcpClient;
            this.stream = this.tcpClient.GetStream();
      
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
                var receivedText = System.Text.Encoding.ASCII.GetString(buffer, 0, receivedBytes);
                dynamic receivedData = JsonConvert.DeserializeObject(receivedText);
                HandleData(receivedData);
                Console.WriteLine(receivedText);
                /*totalBuffer += receivedText;*/
                //string a = JsonConvert.SerializeObject(totalBuffer);
                //testData.Add(a);
                //
                //Console.WriteLine(testData);
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
        /*
         * Handles all the incoming data by looking at the tag in the received package
         */
        private void HandleData(dynamic data)
        {
            JObject jData = data as JObject;
            string tag = jData["Tag"].ToObject<string>();
            switch (tag)
            {
                case "chat":
                    break;
                case "login/register":
                    string name = jData["Data"].ToObject<JObject>()["Name"].ToObject<string>();
                    id = Program.GenerateId(name);
                    byte[] bytes = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(PackageWrapper.WrapWithTag("client/id", new { Id = id })));
                    stream.Write(bytes, 0, bytes.Length);
                    break;
                case "login/client":
                    break;
                case "update/heartrate":
                    break;
                case "update/speed":
                    break;
            }
        }
    }
}