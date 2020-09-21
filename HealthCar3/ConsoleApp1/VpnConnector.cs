using System;
using System.Net.Sockets;
using System.Text;
using Newtonsoft.Json;

namespace ConsoleApp1
{
    
    class VpnConnector
    {
        private string jsonData;
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

        public VpnConnector(JsonSerializerSettings jsonSerializerSettings)
        {
            serializerSettings = jsonSerializerSettings;
            serializerSettings.NullValueHandling = NullValueHandling.Ignore;
            serializerSettings.DefaultValueHandling = DefaultValueHandling.Ignore;
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
            Send(new VpnCommand("session/list"));
        }

        /**
         * Sends a message in the form of a command to the server.
         */
        public void Send(VpnCommand command)
        {
            if (connected)
            {
                Console.WriteLine(JsonConvert.SerializeObject(command, serializerSettings));
                byte[] bytes = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(command, serializerSettings)); //converts the command to bytes.
                try
                {
                    stream = client.GetStream();
                    stream.Write(BitConverter.GetBytes(bytes.Length), 0, 4); //writes the length of the command to the server.
                    stream.Write(bytes, 0, bytes.Length); //writes the message to the server.
                    responseId = command.id;
                    Listen(); //listens for a response.
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
         * Listens for a response from the server.
         */
        public void Listen()
        {
            byte[] lengthBuffer = new byte[4]; //first four bytes indicate length.
            for (int i = 0; i < lengthBuffer.Length; i++)
            {
                lengthBuffer[i] = (byte)stream.ReadByte();
            }
            int bytesize = BitConverter.ToInt32(lengthBuffer); //converts the length to a readable number.

            byte[] bytes = new byte[bytesize];
            while (stream.CanRead) 
            {
                stream.Read(bytes, 0, bytes.Length); //reads the expected response in bytes.
                jsonData = Encoding.ASCII.GetString(bytes); //converts the response bytes to string data.
                parser.Parse(responseId, jsonData); //sends the response to the parser.
            }    
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
    }
}
