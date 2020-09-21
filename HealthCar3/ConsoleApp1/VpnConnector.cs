using System;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Newtonsoft.Json;

namespace ConsoleApp1
{
    
    class VpnConnector
    {
        public string JsonData { get; set; }
        private TcpClient client;
        private MessageParser parser;
        private NetworkStream stream = default;
        private static readonly string address = "145.48.6.10";
        private static readonly int port = 6666;
        private bool connected;
        private int timeoutCounter;
        private readonly int timeoutMax = 3;

        public NetworkStream Stream { get; private set; }

        public VpnConnector()
        {
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
            Send(new Id("session/list"));


        }

        /**
         * Sends a message in the form of a payload to the server.
         */
        public void Send(IPayload payload)
        {
            if (connected)
            {
                byte[] bytes = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(payload)); //converts the payload to bytes.
                try
                {
                    stream = client.GetStream();
                    stream.Write(BitConverter.GetBytes(bytes.Length), 0, 4); //writes the length of the message to the server.
                    stream.Write(bytes, 0, bytes.Length); //writes the message to the server.
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
                int received = stream.Read(bytes, 0, bytes.Length);
                Console.WriteLine(received);
                JsonData = Encoding.ASCII.GetString(bytes);
                parser.getSession();
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
