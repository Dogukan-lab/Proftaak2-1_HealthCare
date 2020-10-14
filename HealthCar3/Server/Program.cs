using Encryption.Shared;
using PackageUtils;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

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
        public static Dictionary<(string name, string password), string> registeredClients; //<(name, password), id>
        public static List<SessionData> savedSession;

        static void Main(string[] args)
        {
            Console.WriteLine("Hello Server!");
            
            savedSession = StorageController.Load();
            
            new Program().Listen();     
        }
        /*
        * Method that accepts the connection to the server
        */
        public void Listen()
        {
            clients = new List<Client>();
            registeredClients = new Dictionary<(string, string), string>();
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
            SessionData cd = new SessionData();
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
            return id;
        }

        /*
         * Sends the message to all the clients connected to the server.
         */
        internal static void Broadcast(string tag, dynamic message)
        {
            foreach (Client client in clients)
                if (client.IsLoggedIn())
                    SendMessageToSpecificClient(client.GetId(), PackageWrapper.SerializeData(tag, message, client.GetEncryptor()));
        }

        /*
         * Sends the message to the given id.
         * Returns true if id is found in the list and false if no match is found.
         */
        internal static bool SendMessageToSpecificClient(string id, byte[] bytes)
        {
            foreach (Client client in clients)
            {
                if (client.GetId() == id)
                {
                    client.GetClientStream().Write(bytes, 0, bytes.Length);
                    return true;
                }
            }
            return false; // No client found with the given id.
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
            return false; // No client found with the given id.
        }

        /*
         * Saves the session for later viewing.
         */
        internal static void SaveSession(Client client)
        {
            savedSession.Add(client.GetSessionData());
            StorageController.Save(savedSession);
        }

        /*
         * Retrieves all the records of the given id.
         */
        internal static dynamic[] GetSession(string id)
        {
            List<dynamic> records = new List<dynamic>();
            foreach(dynamic sd in savedSession)
            {
                if (sd.clientId == id)
                    records.Add(sd);
            }
            if (records.Count > 0)
                return records.ToArray();
            else
                return null; // return null if no records were found with the given id.
        }

        /*
         * Sends a stop message to all the clients with an active session and saves the data
         */
        internal static void EmergencyStop(byte[] bytes)
        {
            foreach(var client in clients)
            {
                if (client.IsSessionActive())
                {
                    // Send stop message to client
                    SendMessageToSpecificClient(client.GetId(), bytes);
                    // Save the data
                    SaveSession(client);
                    // End session server side
                    client.SetSession(false);
                }
            }
        }

        internal static bool ClientLogin(string name, string password)
        {
            return registeredClients.ContainsKey((name, password));
        }

        internal static Encryptor GetTargetClientEncryptor(string id)
        {
            foreach(var client in clients)
            {
                if(client.GetId() == id)
                {
                    return client.GetEncryptor();
                }
            }
            return null;
        }
    }
}
