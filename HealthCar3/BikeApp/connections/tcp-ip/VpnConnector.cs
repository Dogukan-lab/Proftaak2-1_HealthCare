using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using BikeApp.command;
using BikeApp.vr_environment;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BikeApp.connections
{
    /**
     * Connects to the remote server and manages this connection
     */
    internal class VpnConnector
    {
        private dynamic jsonData;
        private CommandCenter commandCenter;
        private readonly JsonSerializerSettings serializerSettings;
        private TcpClient client;
        private readonly MessageParser parser;
        private NetworkStream stream;
        private const string Address = "145.48.6.10";
        private const int Port = 6666;
        private string responseId;
        private int timeoutCounter;
        private const int TimeoutMax = 3;
        public VpnConnector(JsonSerializerSettings jsonSerializerSettings)
        {
            serializerSettings = jsonSerializerSettings;
            serializerSettings.NullValueHandling = NullValueHandling.Ignore;
            parser = new MessageParser(this);
            timeoutCounter = 0;
            commandCenter = new CommandCenter(this);
            Connect();
        }

        /**
         * Connects the client to the server and prints an error on failure.
         */
        private void Connect()
        {
            var listenThread = new Thread(Listen);
            try
            {
                client = new TcpClient();
                client.Connect(Address, Port); //attempts to connect to the VPN server.
                if (!client.Connected) return;
                Send(new { id = "session/list" });
                listenThread.Start();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message); //Writes message on failure.
                timeoutCounter++;
                if (timeoutCounter < TimeoutMax) //Retries the connection up to the given maximum.
                {
                    if (client.Connected)
                    {
                        Console.WriteLine(@"Connection restored. Sending message.");
                    }
                    else
                    {
                        Connect();
                    }
                }
                else
                {
                    Console.WriteLine(@"Connection failed, check connection settings.");
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
                    Console.WriteLine(@"Connection error. Attempting to reconnect."); //if not connected will attempt to reconnect once before failing.
                    Connect();
                }
            }

        }

        /**
         * Read count amount of bytes
         */
        private byte[] ReadTotalBytes(int count)
        {
            var buffer = new byte[count];
            var received = 0;
            while (received < count)
                received += stream.Read(buffer, received, count - received);
            return buffer;
        }

        /**
         * Listens for a response from the server.
         */
        private void Listen()
        {
            while (true)
            {
                var lengthBuffer = ReadTotalBytes(4); //first four bytes indicate length.
                var byteSize = BitConverter.ToInt32(lengthBuffer); //converts the length to a readable number.
                var bytes = ReadTotalBytes(byteSize); //read the all the bytes
                try
                {
                    jsonData = JsonConvert.DeserializeObject(Encoding.ASCII.GetString(bytes), serializerSettings); //converts the response bytes to string data.
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.StackTrace + @"Make sure you connect to the network application.");
                }
                finally
                {
                    // if init command, no callback
                    if (responseId == "session/list" || responseId == "tunnel/create")
                    {
                        parser.Parse(responseId, jsonData); //sends the response to the parser.
                    }
                    else HandleCallBack(jsonData);
                }
            }
        }

        /**
         * Disconnects the client from the server.
         */
        private void Disconnect()
        {
            client.Dispose();
            stream?.Close();
        }

        // Dictionary to keep track of the callbacks
        private readonly Dictionary<int, Action<JObject>> callbacks = new Dictionary<int, Action<JObject>>();
        private int currentSerial;
        public void SendPacket(dynamic data, Action<JObject> callback)
        {
            dynamic packet = new
            {
                id = "tunnel/send",
                data = new
                {
                    dest = parser.GetDestination(), data
                }
            };
            // Send the whole packet
            Send(packet);
            // Add serial to callbacks
            callbacks[CommandUtils.GetSerial()] = callback;
            currentSerial++;
            //Sets the serial for the wrapper method inside of CommandUtils
            CommandUtils.SetSerial(currentSerial);
        }

        /**
         * Find serial and execute callback
         */
        private void HandleCallBack(dynamic data)
        {
            var packetData = data as JObject;

            if (packetData?["data"]?.ToObject<JObject>()?["data"]?.ToObject<JObject>()?["id"]?.ToObject<string>() == "callback")
            {
                //Maybe in the future?
                commandCenter.AttachCamera(true);
                return;
            }

            // Find the matching serial number 
            var receivedSerial = packetData?["data"]?.ToObject<JObject>()?["data"]?.ToObject<JObject>()?["serial"]?.ToObject<int>();
            // Execute the corresponding callback
            if (receivedSerial != null) callbacks[(int) receivedSerial].Invoke(packetData["data"] as JObject);
        }
    }
}
