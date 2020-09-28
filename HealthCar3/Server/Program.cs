using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace Server
{
    class Program
    {
        private TcpListener listener;
        public static Dictionary<Client,int> clients;

        static void Main(string[] args)
        {
            Console.WriteLine("Hello Server!");
            new Program().Listen();
            
        }

        public void Listen()
        {
            clients = new Dictionary<Client, int>();
            //TODO don't know which ip address and port
            listener = new TcpListener(IPAddress.Any, 1330);
            listener.Start();
            listener.BeginAcceptTcpClient(new AsyncCallback(Connect), null);

            Console.ReadLine();
        }

        private void Connect(IAsyncResult ar)
        {
            TcpClient tcpClient = listener.EndAcceptTcpClient(ar);
            ClientData cd = new ClientData();
            Console.WriteLine($"Client connected from {tcpClient.Client.RemoteEndPoint}");
            clients.Add(new Client(tcpClient),clients.Count+1);
            listener.BeginAcceptTcpClient(new AsyncCallback(Connect), null);
        }

        internal static void Disconnect(Client client)
        {
            clients.Remove(client);
            Console.WriteLine("Client disconnected");
        }


    }
}
