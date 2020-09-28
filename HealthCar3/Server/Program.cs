using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace Server
{
    /*
     * Class that allows the client to connect to the server
     */
    class Program
    {
        private TcpListener listener;
        public static Dictionary<Client,int> clients;

        static void Main(string[] args)
        {
            Console.WriteLine("Hello Server!");
            new Program().Listen();
            
        }
        /*
        * Method that accepts the connection to the server
        */
        public void Listen()
        {
            clients = new Dictionary<Client, int>();
            //TODO don't know which ip address and port
            listener = new TcpListener(IPAddress.Any, 1330);
            listener.Start();
            listener.BeginAcceptTcpClient(new AsyncCallback(Connect), null);

            Console.ReadLine();
        }
        /*
        * Method that shows the connection to the server
        */
        private void Connect(IAsyncResult ar)
        {
            TcpClient tcpClient = listener.EndAcceptTcpClient(ar);
            ClientData cd = new ClientData();
            Console.WriteLine($"Client connected from {tcpClient.Client.RemoteEndPoint}");
            clients.Add(new Client(tcpClient),clients.Count+1);
            listener.BeginAcceptTcpClient(new AsyncCallback(Connect), null);
        }
       /*
       * Method that removes the client when it disconnects
       */
        internal static void Disconnect(Client client)
        {
            clients.Remove(client);
            Console.WriteLine("Client disconnected");
        }


    }
}
