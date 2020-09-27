using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using ConsoleApp1.data;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Threading;
using ConsoleApp1.command.scene;
using System.Linq;

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
        private int timeoutCounter;
        private static readonly int timeoutMax = 3;

        public NetworkStream Stream { get; private set; }
        public bool IsConnected() { return client.Connected; }
        public VpnConnector(JsonSerializerSettings jsonSerializerSettings)
        {
            serializerSettings = jsonSerializerSettings;
            serializerSettings.NullValueHandling = NullValueHandling.Ignore;
            parser = new MessageParser(this);
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
                if (client.Connected)
                {
                    Send(new { id = "session/list" });
                    Thread listenThread = new Thread(new ThreadStart(Listen));
                    listenThread.Start();

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message); //Writes message on failure.
                timeoutCounter++;
                if (timeoutCounter < timeoutMax) //Retries the connection up to the given maximum.
                {
                    if (client.Connected)
                    {
                        Console.WriteLine("Connection restored. Sending message.");
                    }
                    else
                    {
                        Connect();
                    }
                }
                else
                {
                    Console.WriteLine("Connection failed, check connection settings.");
                    Disconnect();
                }
            }

        }

        /**
         * Sends a message in the form of a command to the server.
         */
        public void Send(dynamic command)
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
                if (!client.Connected)
                {
                    Console.WriteLine("Connection error. Attempting to reconnect."); //if not connected will attempt to reconnect once before failing.
                    Connect();
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
            while (received < count)
                received += stream.Read(buffer, received, count - received);
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
                //Console.WriteLine(jsonData);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace + "\nMake sure you connect to the network application.");
            }
            finally
            {
                // if init command, no callback
                if (responseId == "session/list" || responseId == "tunnel/create")
                {
                    parser.Parse(responseId, jsonData);//sends the response to the parser.
                }
                else
                    HandleCallBack(jsonData);

                //Console.WriteLine(responseId);
            }
            Listen();
        }

        /**
         * Disconnects the client from the server.
         */
        private void Disconnect()
        {
            client.Dispose();
            if (stream != null)
                stream.Close();
        }

        // Dictionary to keep track of the callbacks
        Dictionary<int, Action<JObject>> callbacks = new Dictionary<int, Action<JObject>>();
        int currentSerial = 0;
        /**
         * Wrap the data in the tunnel and set the destination and serial.
         * Syntax for SendPacket
         * SendPacket("scene/node/add", dynamic, (data) => {
         *     Console.WriteLine("Model got added");
         * });
         */
        public void SendPacket(dynamic data, Action<JObject> callback)
        {
           // Console.WriteLine("CommandUtils.SetSerial(currentSerial): {0}", currentSerial);

            dynamic packet = new
            {
                id = "tunnel/send",
                data = new
                {
                    dest = this.parser.GetDestination(),
                    data = data
                }
            };

            // Send the whole packet
            Send(packet);
            // Add serial to callbacks
            callbacks[CommandUtils.GetSerial()] = callback;
            //callbacks.Add(currentSerial, callback);
            currentSerial++;
            CommandUtils.SetSerial(currentSerial);
        }

        /**
         * Find serial and execute callback
         */
        private void HandleCallBack(dynamic data)
        {
            JObject packetData = data as JObject;

            if (packetData["data"].ToObject<JObject>()["data"].ToObject<JObject>()["id"].ToObject<string>() == "callback")
            {
                //Console.WriteLine("Reached here!!");
                HandleButton(packetData);
                return;
            }

            // Find the matching serial number 
            int receivedSerial = packetData["data"].ToObject<JObject>()["data"].ToObject<JObject>()["serial"].ToObject<int>();
            //Console.WriteLine("Received serial: {0}", receivedSerial);
            // Execute the corresponding callback
            callbacks[receivedSerial].Invoke(packetData["data"] as JObject);
        }

        private void HandleButton(JObject data)
        {
            string button = data["data"].ToObject<JObject>()["data"].ToObject<JObject>()["data"].ToObject<JObject>()["button"].ToObject<string>();
            bool on = data["data"].ToObject<JObject>()["data"].ToObject<JObject>()["data"].ToObject<JObject>()["state"].ToObject<string>() == "on" ? true : false;
            //Console.WriteLine($"Button: {button} State: {on}");
        }
    }
}
