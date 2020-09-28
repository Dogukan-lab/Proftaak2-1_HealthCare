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
        private List<Client> clients;

        static void Main(string[] args)
        {
            Console.WriteLine("Hello Server!");

        }

        public void Listen()
        {
            clients = new List<Client>();
            //TODO
            listener = new TcpListener(IPAddress.Any, 15243);
            listener.Start();
            listener.BeginAcceptTcpClient(new AsyncCallback(Connect), null);

            Console.ReadLine();
        }

        private void Connect(IAsyncResult ar)
        {
            TcpClient tcpClient = listener.EndAcceptTcpClient(ar);
            Console.WriteLine($"Client connected from {tcpClient.Client.RemoteEndPoint}");
            clients.Add(new Client(clients.Count+1, tcpClient));
            listener.BeginAcceptTcpClient(new AsyncCallback(Connect), null);
        }

        internal void Disconnect(Client client)
        {
            clients.Remove(client);
            Console.WriteLine("Client disconnected");
        }

        internal void Broadcast(string packet)
        {
            foreach (var client in clients)
            {
                client.Write(packet);
            }
        }

        internal void SendToUser(string user, string packet)
        {
            foreach (var client in clients.Where(c => c.UserName == user))
            {
                client.Write(packet);
            }
        }
    }
}
