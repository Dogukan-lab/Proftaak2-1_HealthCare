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
            string tag = jData["tag"].ToObject<string>();
            switch (tag)
            {
                case "chat/message":
                case "chat/broadcast":
                    string message = jData["data"].ToObject<JObject>()["message"].ToObject<string>();
                    Console.WriteLine($"Received Message: {message}");
                    break;
                case "client/register/success":
                    Console.WriteLine($"Received Id: {jData["data"].ToObject<JObject>()["clientId"].ToObject<string>()}");
                    uniqueId = jData["data"].ToObject<JObject>()["clientId"].ToObject<string>();
                    break;
                case "client/register/error":
                    Console.WriteLine($"ERROR: {jData["data"].ToObject<JObject>()["message"].ToObject<string>()}");
                    break;
                case "chat/message/success":
                    Console.WriteLine($"Succes: {jData["data"].ToObject<JObject>()["message"].ToObject<string>()}");
                    break;
                case "chat/message/error":
                    Console.WriteLine($"ERROR: {jData["data"].ToObject<JObject>()["message"].ToObject<string>()}");
                    break;
                case "chat/broadcast/success":
                    Console.WriteLine($"Succes: {jData["data"].ToObject<JObject>()["message"].ToObject<string>()}");
                    break;
                case "chat/broadcast/error":
                    Console.WriteLine($"ERROR: {jData["data"].ToObject<JObject>()["message"].ToObject<string>()}");
                    break;
            }
        }

        private void OnConnected()
        {
            connected = true;
            stream.BeginRead(buffer, 0, buffer.Length, new AsyncCallback(OnRead), null);
        }

        public void ChatTest(string id, string message)
        {
            byte[] bytes = PackageWrapper.SerializeData("chat/message", new { destination = id, message = message});

            stream.Write(bytes, 0, bytes.Length);
        }
        public void BroadcastTest(string message)
        {
            byte[] bytes = PackageWrapper.SerializeData("chat/broadcast", new { message = message });

            stream.Write(bytes, 0, bytes.Length);
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
        public void RegisterToServer(string name)
        {
            byte[] bytes = PackageWrapper.SerializeData("client/register", new { name = name });

            stream.Write(bytes, 0, bytes.Length);
        }

        public void LoginToServer(string name, string id)
        {
            byte[] bytes = PackageWrapper.SerializeData("client/login", new { name = name, clientId = id});

            stream.Write(bytes, 0, bytes.Length);
        }

        /*
         * Method used to send messages to the server.
         */
        public void UpdateHeartRate(int heartRate)
        {
            byte[] bytes = PackageWrapper.SerializeData("update/heartrate", new { Id = uniqueId, HeartRate = heartRate});

            if(connected)
                stream.Write(bytes, 0, bytes.Length);
        }
        public void UpdateSpeed(float speed)
        {
            byte[] bytes = PackageWrapper.SerializeData("update/speed", new { Id = uniqueId, Speed = speed});

            if(connected)
                stream.Write(bytes, 0, bytes.Length);
        }
    }
}
