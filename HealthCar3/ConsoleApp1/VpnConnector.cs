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

        public NetworkStream Stream { get; private set; }

        public VpnConnector()
        {
            parser = new MessageParser(this);
            connected = false;
            Connect();
        }

        private void Connect()
        {
            try
            {
                client = new TcpClient();
                client.Connect(address, port);
                connected = true;
            } catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void Send(IPayload payload)
        {
            if (connected)
            {
                byte[] bytes = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(payload));
                string message = JsonConvert.SerializeObject(payload);
                //Console.WriteLine(message);
                try
                {
                    stream = client.GetStream();
                    stream.Write(BitConverter.GetBytes(bytes.Length), 0, 4);
                    stream.Write(bytes, 0, bytes.Length);
                    Listen();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            } else
            {
                Console.WriteLine("Connection error. Attempting to reconnect.");
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

        public void Listen()
        {
            byte[] lengthBuffer = new byte[4];
            for (int i = 0; i < lengthBuffer.Length; i++)
            {
                lengthBuffer[i] = (byte)stream.ReadByte();
            }
            int bytesize = BitConverter.ToInt32(lengthBuffer);

            byte[] bytes = new byte[bytesize];
            while (stream.CanRead)
            {
                //temp code, needs to be amplified with overflow protection, message buffering and parser calls.
                int received = stream.Read(bytes, 0, bytes.Length);
                Console.WriteLine(received);
                JsonData = Encoding.ASCII.GetString(bytes);
                parser.getSession();
            }    
        }

    }
}
