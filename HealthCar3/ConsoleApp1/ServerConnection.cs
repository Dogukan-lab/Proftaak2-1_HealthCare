using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Security.Cryptography;
using Newtonsoft.Json;
using System.Threading;
using Newtonsoft.Json.Linq;
using PackageUtils;

namespace ConsoleApp1
{
    class ServerConnection
    {
        private TcpClient clientConnection;
        private NetworkStream stream;
        private String IPAddress = "127.0.0.1";
        private int port = 1330;
        private int totalTries;
        private readonly int MAXRECONTRIES = 3;
        private string uniqueId = "";
        private byte[] buffer = new byte[1024];
        private bool connected = false;
        
        public ServerConnection()
        {
            clientConnection = new TcpClient();

            Connect(IPAddress, port);
        }
        public ServerConnection(String IPAddress, int port)
        {
            clientConnection = new TcpClient();

            Connect(IPAddress, port);
        }

        /*
         * Method used to make a connection with the server.
         */
        private void Connect(string IPAddress, int port)
        {
            try
            {
                clientConnection.Connect(IPAddress, port);
                stream = clientConnection.GetStream();

                if (clientConnection.Connected)
                {
                    OnConnected();
                }
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex);
                if (!clientConnection.Connected && totalTries < 3)
                {
                    Connect(IPAddress, port);
                }
                else
                {
                    OnDisconnect();
                }
            }
        }

        private void OnRead(IAsyncResult ar)
        {
            int receivedBytes = stream.EndRead(ar);
            string receivedText = System.Text.Encoding.ASCII.GetString(buffer, 0, receivedBytes);
            dynamic receivedData = JsonConvert.DeserializeObject(receivedText);
            HandleData(receivedData);
            stream.BeginRead(buffer, 0, buffer.Length, new AsyncCallback(OnRead), null);
        }

        private void HandleData(dynamic data)
        {
            JObject jData = data as JObject;
            string tag = jData["Tag"].ToObject<string>();
            switch (tag)
            {
                case "broadcast":
                    Console.WriteLine($"Received Message: {jData["Data"].ToObject<JObject>()["Message"].ToObject<string>()}");
                    break;
                case "client/id":
                    Console.WriteLine($"Received Id: {jData["Data"].ToObject<JObject>()["Id"].ToObject<string>()}");
                    uniqueId = jData["Data"].ToObject<JObject>()["Id"].ToObject<string>();
                    break;
            }
        }

        private void OnConnected()
        {
            connected = true;
            stream.BeginRead(buffer, 0, buffer.Length, new AsyncCallback(OnRead), null);
            RegisterToServer("kees banaan");
        }

        /*
         * Method used to disconnect from the server.
         */
        private void OnDisconnect()
        {
            stream.Dispose();
            clientConnection.Close();
        }

        /*
         * Method used to register to the server
         */
        private void RegisterToServer(string name)
        {
            byte[] bytes = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(PackageWrapper.WrapWithTag("login/register", new { Name = name })));

            stream.Write(bytes, 0, bytes.Length);
        }

        /*
         * Method used to send messages to the server.
         */
        public void UpdateHeartRate(int heartRate)
        {
            byte[] bytes = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(PackageWrapper.WrapWithTag("update/heartrate", new { Id = uniqueId, HeartRate = heartRate})));

            if(connected)
                stream.Write(bytes, 0, bytes.Length);
        }
        public void UpdateSpeed(float speed)
        {
            byte[] bytes = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(PackageWrapper.WrapWithTag("update/speed", new { Id = uniqueId, Speed = speed})));

            if(connected)
                stream.Write(bytes, 0, bytes.Length);
        }
    }
}
