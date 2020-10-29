using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using Encryption.Shared;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PackageUtils;

namespace DocterApplication
{
    public class ServerConnection
    {
        private const string IpAddress = "127.0.0.1";
        private const int Port = 1330;
        private const int MaxReconTries = 3;
        private readonly byte[] buffer = new byte[4];
        private readonly TcpClient clientConnection;
        private readonly Decryptor decryptor;
        private readonly Encryptor encryptor;
        private bool connected;
        private bool keyExchanged;
        private Layout layoutParent;
        private bool loggedIn;
        private bool receivedLoginFeedback;
        private readonly List<SessionData> records = new List<SessionData>();
        private bool retreivedRecords;
        private NetworkStream stream;
        private int totalTries;

        public ServerConnection()
        {
            clientConnection = new TcpClient();
            encryptor = new Encryptor();
            decryptor = new Decryptor();
            Connect(IpAddress, Port);
        }

        public bool IsConnected()
        {
            return connected;
        }
        public bool IsLoggedIn()
        {
            return loggedIn;
        }
        public bool HasReceivedLoginFeedback()
        {
            return receivedLoginFeedback;
        }
        public void SetLayoutParent(Layout parent)
        {
            layoutParent = parent;
        }
        public bool HasRetreivedRecords()
        {
            return retreivedRecords;
        }

        /*
         * Method used to make a connection with the server.
         */
        private void Connect(string ipAddress, int port)
        {
            try
            {
                clientConnection.Connect(ipAddress, port);
                stream = clientConnection.GetStream();

                if (clientConnection.Connected) OnConnected();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                if (!clientConnection.Connected && totalTries < MaxReconTries)
                {
                    totalTries++;
                    Connect(ipAddress, port);
                }
                else
                {
                    OnDisconnect();
                }
            }
        }

        /**
         * Read count amount of bytes
         */
        private byte[] ReadTotalBytes(int count)
        {
            var buffer = new byte[count];
            var received = 0;
            while (received < count)
                received += stream.Read(buffer, received, count - received);
            return buffer;
        }

        /**
         * Async callback that gets called when we have read something from the stream.
         */
        private void OnRead(IAsyncResult ar)
        {
            try
            {
                var lengthPreFix = BitConverter.ToInt32(buffer);
                var receivedBytes = ReadTotalBytes(lengthPreFix);
                dynamic receivedData;

                if (keyExchanged)
                {
                    receivedData = JsonConvert.DeserializeObject(decryptor.DecryptAes(receivedBytes, 0, lengthPreFix));
                }
                else
                {
                    var receivedText = Encoding.ASCII.GetString(receivedBytes, 0, lengthPreFix);
                    receivedData = JsonConvert.DeserializeObject(receivedText);
                }

                HandleData(receivedData);
                stream.BeginRead(buffer, 0, buffer.Length, OnRead, null);
            }
            catch (IOException)
            {
                OnDisconnect();
            }
        }

        /**
         * Decode the message by switching on the tag in the package.
         */
        private void HandleData(dynamic data)
        {
            var jData = data as JObject;
            var tag = jData?["tag"]?.ToObject<string>();
            switch (tag)
            {
                case "encrypt/key/success": // Initializes the new received encryption keys.
                    byte[] key = decryptor.DecryptRsa(data.data.key.ToObject<byte[]>());
                    byte[] iv = decryptor.DecryptRsa(data.data.iv.ToObject<byte[]>());

                    encryptor.AesKey = key;
                    encryptor.AesIv = iv;

                    decryptor.AesKey = key;
                    decryptor.AesIv = iv;

                    keyExchanged = true;
                    break;
                case "doctor/login/success": // Logs the doctor in
                    receivedLoginFeedback = true;
                    loggedIn = true;
                    break;
                case "doctor/login/error": // Lets the doctor know that the input credentials were incorrect.
                    receivedLoginFeedback = true;
                    break;
                case "client/update/heartRate": // Updates the heart rate of a connected client.
                    layoutParent.NewHeartRate((string)data.data.clientId, (int)data.data.heartRate);
                    break;
                case "client/update/speed": // Updates the speed of a connected client.
                    layoutParent.NewSpeed((string)data.data.clientId, (int)data.data.speed);
                    break;
                case "doctor/newClient": // Lets this application know a new client has connected to the server.
                    layoutParent.NewClient((string)data.data.clientId, (string)data.data.name);
                    break;
                case "client/disconnect": // Lets this application know a client has disconnected from the server.
                    layoutParent.RemoveClient((string)data.data.clientId);
                    break;
                case "doctor/getSessions/fragment": // Received a fragment of the records list from server.
                    foreach (dynamic r in ((JArray) data.data.records).Children()) records.Add(new SessionData(r));
                    GetNextFragment();
                    break;
                case "doctor/getSessions/success":// Received the final fragment of the records list from server.
                    foreach (dynamic r in ((JArray) data.data.records).Children()) records.Add(new SessionData(r));
                    layoutParent.RefreshHistoryPage(records);
                    retreivedRecords = true;
                    break;
                // Unused responses.
                case "chat/message/success":
                case "chat/broadcast/success":
                case "session/resistance/success":
                case "session/start/success":
                case "session/stop/success":
                    Console.WriteLine(
                        $@"Success: {jData["data"]?.ToObject<JObject>()?["message"]?.ToObject<string>()}");
                    break;
                case "doctor/clientHistory/error":
                case "chat/message/error":
                case "chat/broadcast/error":
                case "session/resistance/error":
                case "session/start/error":
                case "session/stop/error":
                    Console.WriteLine($@"ERROR: {jData["data"]?.ToObject<JObject>()?["message"]?.ToObject<string>()}");
                    break;
            }
        }

        /*
         * Gets called when the application has succesfully connected to the server.
         */
        private void OnConnected()
        {
            connected = true;
            InitializeRsa();
            stream.BeginRead(buffer, 0, buffer.Length, OnRead, null);
        }

        /*
         * Method used to disconnect from the server.
         */
        private void OnDisconnect()
        {
            stream.Dispose();
            clientConnection.Close();
        }

        /*
         * Method used to login to the server
         */
        public void LoginToServer(string username, string password)
        {
            receivedLoginFeedback = false;
            var bytes = PackageWrapper.SerializeData("doctor/login", new {username, password}, encryptor);

            stream.Write(bytes, 0, bytes.Length);
        }

        private void InitializeRsa()
        {
            var (privateKey, pubKey) = encryptor.GenerateRsaKey();
            decryptor.RsaPrivateKey = privateKey;

            var bytes = PackageWrapper.SerializeData
            (
                "encrypt/key",
                new
                {
                    exponent = pubKey.Exponent,
                    modulus = pubKey.Modulus
                }
            );

            stream.Write(bytes, 0, bytes.Length);
        }
        #region // Writer functions

        /*
         * Sends a message to a specific client.
         */
        public void Chat(string id, string message)
        {
            var bytes = PackageWrapper.SerializeData("chat/message", new {clientId = id, message}, encryptor);

            stream.Write(bytes, 0, bytes.Length);
        }

        /*
         * Sends a message to all the current connected clients.
         */
        public void Broadcast(string message)
        {
            var bytes = PackageWrapper.SerializeData("chat/broadcast", new {message}, encryptor);

            stream.Write(bytes, 0, bytes.Length);
        }

        /*
         * Sends a new resistance to the given client.
         */
        public void SetNewResistance(string id, string resistance)
        {
            var bytes = PackageWrapper.SerializeData("session/resistance", new {clientId = id, resistance}, encryptor);

            stream.Write(bytes, 0, bytes.Length);
        }

        /*
         * Starts the the session of the given client.
         */
        public void StartSession(string id)
        {
            var bytes = PackageWrapper.SerializeData("session/start", new {clientId = id}, encryptor);

            stream.Write(bytes, 0, bytes.Length);
        }

        /*
         * Stops the the session of the given client.
         */
        public void StopSession(string id)
        {
            var bytes = PackageWrapper.SerializeData("session/stop", new {clientId = id}, encryptor);

            stream.Write(bytes, 0, bytes.Length);
        }
        /*
         * Stops all the active clients
         */
        public void EmergencyStopSessions()
        {
            var bytes = PackageWrapper.SerializeData("session/emergencyStop", new { }, encryptor);

            stream.Write(bytes, 0, bytes.Length);
        }

        /*
         * Retrieves all the records from sessions of the past.
         */
        public void GetSessions()
        {
            retreivedRecords = false;
            records.Clear();
            var bytes = PackageWrapper.SerializeData("doctor/getSessions", new { }, encryptor);

            stream.Write(bytes, 0, bytes.Length);
        }

        public void GetNextFragment()
        {
            var bytes = PackageWrapper.SerializeData("doctor/getSessions/nextFragment", new { }, encryptor);

            stream.Write(bytes, 0, bytes.Length);
        }

        #endregion
    }
}