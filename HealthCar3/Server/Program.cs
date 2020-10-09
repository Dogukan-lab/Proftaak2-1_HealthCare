using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;

namespace Server
{
    /*
     * Class that allows the client to connect to the server
     * :)
     */
    class Program
    {
        private TcpListener listener;
        public static List<Client> clients;
        public static Dictionary<string, string> registeredClients;

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
            clients = new List<Client>();
            registeredClients = new Dictionary<string, string>();
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
            clients.Add(new Client(tcpClient));
            listener.BeginAcceptTcpClient(new AsyncCallback(Connect), null);
        }
       /*
       * Method that removes the client when it disconnects
       */
        internal static void Disconnect(Client client)
        {
            clients.Remove(client);
            client.GetClientStream().Close();           
            Console.WriteLine($"Client {client.GetId()} disconnected");
        }

        /*
         * Generates an id for the new registered client
         */
        public static string GenerateId(string name)
        {
            Random random = new Random();
            string id = "";
            for(int i = 0; i < 10; i++)
            {
                id += random.Next(0, 9);
            }
            registeredClients.Add(id, name);
            return id;
        }

        /*
         * Sends the message to all the clients connected to the server.
         */
        public static void Broadcast(byte[] bytes)
        {
            foreach(Client client in clients)
            {
                client.GetClientStream().Write(bytes, 0, bytes.Length);
            }
        }

        /*
         * Sends the message to the given id.
         * Returns true if id is found in the list and false if no match is found.
         */
        public static bool SendMessageToSpecificClient(string id, byte[] bytes)
        {
            foreach (Client client in clients)
            {
                if (client.GetId() == id)
                {
                    client.GetClientStream().Write(bytes, 0, bytes.Length);
                    return true;
                }
            }
            return false;
        }

        internal static bool ActiveSession(string id, out Client targetClient)
        {
            targetClient = null;
            foreach(Client client in clients)
            {
                if(client.GetId() == id)
                {
                    targetClient = client;
                    return client.IsSessionActive();
                }
            }
            return false;
        }
    }
}
