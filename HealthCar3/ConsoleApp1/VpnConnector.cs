using System;
using System.Net.Sockets;
using System.Text;
using Newtonsoft.Json;

namespace ConsoleApp1
{
    
    class VpnConnector
    {
        private TcpClient client;
        private MessageParser parser;
        private NetworkStream stream = default;
        private static readonly string address = "145.48.6.10";
        private static readonly int port = 6666;
        private Boolean connected;

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

        public void Send(Payload payload)
        {
            if (connected)
            {
                byte[] bytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(payload));
                String message = JsonConvert.SerializeObject(payload);
                Console.WriteLine(message);
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

        private void Listen()
        {
            const int bytesize = 4096;
            byte[] bytes = new byte[bytesize];
            while (stream.CanRead)
            {
                //temp code, needs to be amplified with overflow protection, message buffering and parser calls.
                int received = stream.Read(bytes, 0, bytes.Length);
                Console.WriteLine(received);
                Console.WriteLine(Encoding.UTF8.GetString(bytes));
            }
        }

    }
}
