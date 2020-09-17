using System;
using System.Net.Sockets;
using System.Text;
using Newtonsoft.Json;

namespace ConsoleApp1
{
    
    class VpnConnector
    {
        private TcpClient client;
        private NetworkStream stream = default;
        private static readonly string address = "145.48.6.10";
        private static readonly int port = 6666;

        public NetworkStream Stream { get; private set; }

        public VpnConnector()
        {
            Connect();
        }

        private void Connect()
        {
            try
            {
                client = new TcpClient();
                client.Connect(address, port);
            } catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void Send(Payload payload)
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
            } catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            
        }

        private void Listen()
        {
            const int bytesize = 4096;
            byte[] bytes = new byte[bytesize];
            while (stream.CanRead)
            {
                int received = stream.Read(bytes, 0, bytes.Length);
                Console.WriteLine(received);
                Console.WriteLine(Encoding.Unicode.GetString(bytes));
            }
        }

    }
}
