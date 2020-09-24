using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using ConsoleApp1.data;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ConsoleApp1
{
    
    class VpnConnector
    {
        private dynamic jsonData;
        private JsonSerializerSettings serializerSettings;
        private TcpClient client;
        private MessageParser parser;
        private NetworkStream stream = default;
        private static readonly string address = "145.48.6.10";
        private static readonly int port = 6666;
        private string responseId;
        private bool connected;
        private int timeoutCounter;
        private readonly int timeoutMax = 3;

        public NetworkStream Stream { get; private set; }
        public bool IsConnected() { return this.connected; }
        public VpnConnector(JsonSerializerSettings jsonSerializerSettings)
        {
            serializerSettings = jsonSerializerSettings;
            serializerSettings.NullValueHandling = NullValueHandling.Ignore;
            parser = new MessageParser(this);
            connected = false;
            timeoutCounter = 0;
            Connect();
        }

        /**
         * Connects the client to the server and prints an error on failure.
         */
        private void Connect()
        {
            try
            {
                client = new TcpClient();
                client.Connect(address, port); //attempts to connect to the VPN server.
                connected = true;
            } catch (Exception ex)
            {
                Console.WriteLine(ex.Message); //Writes message on failure.
                timeoutCounter++;
                if (timeoutCounter < timeoutMax) //Retries the connection up to the given maximum.
                {
                    Connect();
                } else
                {
                    Disconnect();
                }
            }
            Send(new { id = "session/list" });
        }

        /**
         * Sends a message in the form of a command to the server.
         */
        public void Send(dynamic command)
        {
            if (connected)
            {
                byte[] bytes = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(command, serializerSettings)); //converts the command to bytes.
                try
                {
                    stream = client.GetStream();
                    stream.Write(BitConverter.GetBytes(bytes.Length), 0, 4); //writes the length of the command to the server.
                    stream.Write(bytes, 0, bytes.Length); //writes the message to the server.
                    responseId = command.id;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            } else
            {
                Console.WriteLine("Connection error. Attempting to reconnect."); //if not connected will attempt to reconnect once before failing.
                Connect();
                if (connected)
                {
                    Console.WriteLine("Connection restored. Sending message.");
                } else
                {
                    Console.WriteLine("Reconnection failed, check connection settings.");
                }
            }  
        }

        /**
         * Read count amount of bytes
         */
        byte[] readTotalBytes(int count)
        {
            byte[] buffer = new byte[count];
            int received = 0;
            while(received < count)
                received += stream.Read(buffer, received, count-received);
            return buffer;
        }

        /**
         * Listens for a response from the server.
         */
        public void Listen()
        {
            byte[] lengthBuffer = readTotalBytes(4); //first four bytes indicate length.

            int bytesize = BitConverter.ToInt32(lengthBuffer); //converts the length to a readable number.

            byte[] bytes = readTotalBytes(bytesize); //read the all the bytes
            try
            {
                jsonData = JsonConvert.DeserializeObject(Encoding.ASCII.GetString(bytes), serializerSettings); //converts the response bytes to string data.
                Console.WriteLine(jsonData);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace + "\nMake sure you connect to the network application.");
            }
            finally
            {
                // if init command, no callback
                if (responseId == "session/list" || responseId == "tunnel/create") 
                    parser.Parse(responseId, jsonData); //sends the response to the parser.
                else
                    HandleCallBack(jsonData);
            }
            Listen();
        }

        /**
         * Disconnects the client from the server.
         */
        private void Disconnect()
        {
            client.Dispose();
            stream.Close();
            connected = false;
        }

        // Dictionary to keep track of the callbacks
        Dictionary<int, Action<JObject>> callbacks = new Dictionary<int, Action<JObject>>();
        int currentSerial = 1;
        /**
         * Wrap the data in the tunnel and set the destination and serial.
         * Syntax for SendPacket
         * SendPacket("scene/node/add", new { transform = new[] { } }, (data) => {
         *     Console.WriteLine("Model got added");
         * });
         */
        public void SendPacket(string id, dynamic data, Action<JObject> callback)
        {
            dynamic packet = new
            {
                id = "tunnel/send",
                data = new
                {
                    dest = this.parser.GetDestination(),
                    data = new
                    {
                        id = id,
                        serial = currentSerial,
                        data = data
                    }
                }
            };

            // Send the whole packet
            Send(packet);
            // Add serial to callbacks
            callbacks[currentSerial] = callback;
            currentSerial++;
        }

        /**
         * Find serial and execute callback
         */
        private void HandleCallBack(dynamic data)
        {
            JObject packetData = data as JObject;

            if (packetData["data"].ToObject<JObject>()["data"].ToObject<JObject>()["id"].ToObject<string>() == "callback")
                return;

            // Find the matching serial number 
            int receivedSerial = packetData["data"].ToObject<JObject>()["data"].ToObject<JObject>()["serial"].ToObject<int>();
            // Execute the corresponding callback
            callbacks[receivedSerial].Invoke(packetData["data"] as JObject);
        }       
    }
}
