﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using Encryption.Shared;
using PackageUtils;

namespace Server
{
    /*
     * Class that allows the client to connect to the server
     */
    public class Program
    {
        private static List<Client> clients;
        public static Dictionary<(string name, string password), string> registeredClients; //<(name, password), id>
        private static List<SessionData> savedSession;
        public static List<ClientCredentials> savedClientData;
        public static Client doctorClient;
        private static List<dynamic> tempRecords;
        private TcpListener listener;

        private static void Main(string[] args)
        {
            Console.WriteLine("Hello Server!");
            savedSession = StorageController.Load();
            savedClientData = StorageController.LoadClientData();
            registeredClients = SetRegisteredClients();
            new Program().Listen();
        }

        /*
         * Inits the registered clients dictionary to check for log in attempts
         */
        private static Dictionary<(string name, string password), string> SetRegisteredClients()
        {
            var registeredClients = new Dictionary<(string name, string password), string>();
            if (savedClientData != null || savedClientData!.Any())
                foreach (var item in savedClientData)
                    if (!registeredClients.ContainsKey((item.username, item.password)))
                        registeredClients.Add((item.username, item.password), item.id);

            return registeredClients;
        }

        /*
        * Method that accepts the connection to the server
        */
        public void Listen()
        {
            clients = new List<Client>();
            listener = new TcpListener(IPAddress.Any, 1330);

            listener.Start();
            listener.BeginAcceptTcpClient(Connect, null);
            Console.ReadLine();
        }

        /*
        * Method that shows the connection to the server
        */
        private void Connect(IAsyncResult ar)
        {
            var tcpClient = listener.EndAcceptTcpClient(ar);
            Console.WriteLine($"Client connected from {tcpClient.Client.RemoteEndPoint}");
            clients.Add(new Client(tcpClient));
            listener.BeginAcceptTcpClient(Connect, null);
        }

        /*
         * Method that removes the client when it disconnects
         */
        internal static void Disconnect(Client client)
        {
            if (client == doctorClient)
            {
                doctorClient = null;
            }
            else if (doctorClient != null)
            {
                var bytes = PackageWrapper.SerializeData("client/disconnect", new {clientId = client.GetId()},
                    doctorClient.GetEncryptor());
                doctorClient.GetClientStream().Write(bytes, 0, bytes.Length);
            }

            clients.Remove(client);
            client.GetClientStream().Close();
            Console.WriteLine($"Client {client.GetId()} disconnected");
        }

        /*
         * Generates an id for the new registered client
         */
        public static string GenerateId(string name)
        {
            var random = new Random();
            var id = "";
            for (var i = 0; i < 10; i++) id += random.Next(0, 9);
            return id;
        }

        /*
         * Sends the message to all the clients connected to the server.
         */
        internal static void Broadcast(string tag, dynamic message)
        {
            foreach (var client in clients)
                if (client.IsLoggedIn())
                    SendMessageToSpecificClient(client.GetId(),
                        PackageWrapper.SerializeData(tag, message, client.GetEncryptor()));
        }

        /*
         * Sends the message to the given id.
         * Returns true if id is found in the list and false if no match is found.
         */
        internal static bool SendMessageToSpecificClient(string id, byte[] bytes)
        {
            foreach (var client in clients)
                if (client.GetId() == id)
                {
                    client.GetClientStream().Write(bytes, 0, bytes.Length);
                    return true;
                }

            //No client found with the given id.
            return false;
        }

        /*
        * Saves the clientData for later viewing.
        */
        internal static void SaveClientData(Client client)
        {
            savedClientData.Add(client.GetClientCredentials());
            StorageController.SaveClientCredentials(savedClientData);
        }

        internal static bool ActiveSession(string id, out Client targetClient)
        {
            targetClient = null;
            foreach (var client in clients)
                if (client.GetId() == id)
                {
                    targetClient = client;
                    return client.IsSessionActive();
                }

            //No client found with the given id.
            return false;
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
            var records = new List<dynamic>();
            foreach (dynamic sd in savedSession)
                if (sd.clientId == id)
                    records.Add(sd);
            if (records.Count > 0)
                return records.ToArray();

            // return null if no records were found with the given id.
            return null;
        }

        /*
         * Sends a stop message to all the clients with an active session and saves the data
         */
        internal static void EmergencyStop()
        {
            foreach (var client in clients)
                if (client.IsSessionActive())
                {
                    // Send stop message to client
                    //SendMessageToSpecificClient(client.GetId(), bytes);
                    var bytes = PackageWrapper.SerializeData("session/stop", new { }, client.GetEncryptor());
                    client.GetClientStream().Write(bytes, 0, bytes.Length);
                    // Save the data
                    SaveSession(client);
                    // End session server side
                    client.SetSession(false);
                }
        }

        /*
         * Checks if the client already has logged in.
         */
        internal static bool ClientLogin(string name, string password)
        {
            return registeredClients.ContainsKey((name, password));
        }
        
        /*
         * Finds the client encryption of the current client.
         * Think of it as peer to peer.
         */
        internal static Encryptor GetTargetClientEncryptor(string id)
        {
            foreach (var client in clients)
                if (client.GetId() == id)
                    return client.GetEncryptor();
            return null;
        }

        internal static void NotifyDoctor(string id, string name)
        {
            if (doctorClient != null)
            {
                var bytes = PackageWrapper.SerializeData("doctor/newClient", new {clientId = id, name},
                    doctorClient.GetEncryptor());
                doctorClient.GetClientStream().Write(bytes, 0, bytes.Length);
            }
        }

        internal static void RetrieveAllRecords()
        {
            tempRecords = savedSession.OfType<dynamic>().ToList();
            SendNextFragment();
        }

        internal static void SendNextFragment()
        {
            byte[] bytes;
            if (tempRecords.Count > 2)
            {
                dynamic[] nextRecords = {tempRecords[0], tempRecords[1]};
                bytes = PackageWrapper.SerializeData("doctor/getSessions/fragment", new {records = nextRecords},
                    doctorClient.GetEncryptor());
                doctorClient.GetClientStream().Write(bytes, 0, bytes.Length);
                tempRecords.RemoveAt(0);
                tempRecords.RemoveAt(0);
            }
            else
            {
                bytes = PackageWrapper.SerializeData("doctor/getSessions/success", new {records = tempRecords},
                    doctorClient.GetEncryptor());
                doctorClient.GetClientStream().Write(bytes, 0, bytes.Length);
                tempRecords.Clear();
            }
        }
    }
}