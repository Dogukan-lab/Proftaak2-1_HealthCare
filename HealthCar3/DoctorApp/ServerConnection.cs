using System;
using System.Text;
using System.Net.Sockets;
using System.Security.Cryptography;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PackageUtils;
using System.IO;
using Encryption.Shared;

namespace DoctorApp
{
    class ServerConnection
    {
        private TcpClient clientConnection;
        private NetworkStream stream;
        private String IPAddress = "127.0.0.1";
        private int port = 1330;
        private int totalTries;
        private readonly int MAXRECONTRIES = 3;
        private string uniqueId = "";
        private byte[] buffer = new byte[1024];
        private bool connected = false;
        private bool loggedIn = false;
        private bool keyExchanged = false;
        private Encryptor encryptor;
        private Decryptor decryptor;
        public bool isConnected() { return this.connected; }
        public bool isLoggedIn() { return this.loggedIn; }

        public ServerConnection()
        {
            clientConnection = new TcpClient();
            encryptor = new Encryptor();
            decryptor = new Decryptor();
            Connect(IPAddress, port);
        }

        /*
         * Method used to make a connection with the server.
         */
        private void Connect(string IPAddress, int port)
        {
            try
            {
                clientConnection.Connect(IPAddress, port);
                stream = clientConnection.GetStream();

                if (clientConnection.Connected)
                {
                    OnConnected();
                }
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex);
                if (!clientConnection.Connected && totalTries < MAXRECONTRIES)
                {
                    totalTries++;
                    Connect(IPAddress, port);
                }
                else
                {
                    OnDisconnect();
                }
            }
        }

        private void OnRead(IAsyncResult ar)
        {
            try
            {
                int receivedBytes = stream.EndRead(ar);
                dynamic receivedData;
                
                if (keyExchanged)
                {
                    receivedData = JsonConvert.DeserializeObject(decryptor.DecryptAES(buffer, 0, receivedBytes));
                }
                else
                {
                    string receivedText = Encoding.ASCII.GetString(buffer, 0, receivedBytes);
                    receivedData = JsonConvert.DeserializeObject(receivedText);
                }
                HandleData(receivedData);
                stream.BeginRead(buffer, 0, buffer.Length, new AsyncCallback(OnRead), null);
            }
            catch (IOException)
            {
                OnDisconnect();
                return;
            }
        }

        private void HandleData(dynamic data)
        {
            JObject jData = data as JObject;
            string tag = jData["tag"].ToObject<string>();
            switch (tag)
            {
                case "encrypt/key/success":
                    byte[] key = decryptor.DecryptRSA(data.data.key.ToObject<byte[]>());
                    byte[] iv = decryptor.DecryptRSA(data.data.iv.ToObject<byte[]>());

                    encryptor.AesKey = key;
                    encryptor.AesIv = iv;

                    decryptor.AesKey = key;
                    decryptor.AesIv = iv;

                    keyExchanged = true;
                    break;
                case "doctor/login/success":
                    loggedIn = true;
                    Console.WriteLine(jData["data"].ToObject<JObject>()["message"].ToObject<string>());
                    break;
                case "doctor/login/error":
                    Console.WriteLine(jData["data"].ToObject<JObject>()["message"].ToObject<string>());
                    break;
                case "client/update/heartRate":
                    //Console.WriteLine($"{jData["data"].ToObject<JObject>()["clientId"].ToObject<string>()}: {jData["data"].ToObject<JObject>()["heartRate"].ToObject<string>()} BPM");
                    break;
                case "client/update/speed":
                    //Console.WriteLine($"{jData["data"].ToObject<JObject>()["clientId"].ToObject<string>()}: {jData["data"].ToObject<JObject>()["speed"].ToObject<string>()} m/s");
                    break;
                case "doctor/clientHistory/success":
                    Console.WriteLine($"{jData["data"]}");
                    break;
                case "chat/message/success":
                case "chat/broadcast/success":
                case "session/resistance/success":
                case "session/start/success":
                case "session/stop/success":
                    Console.WriteLine($"Succes: {jData["data"].ToObject<JObject>()["message"].ToObject<string>()}");
                    break;
                case "doctor/clientHistory/error":
                case "chat/message/error":
                case "chat/broadcast/error":
                case "session/resistance/error":
                case "session/start/error":
                case "session/stop/error":
                    Console.WriteLine($"ERROR: {jData["data"].ToObject<JObject>()["message"].ToObject<string>()}");
                    break;
            }
        }

        private void OnConnected()
        {
            connected = true;
            InitializeRsa();
            stream.BeginRead(buffer, 0, buffer.Length, new AsyncCallback(OnRead), null);
        }

        #region // Writer functions
        /*
         * Sends a message to a specific client.
         */
        public void Chat(string id, string message)
        {
            byte[] bytes = PackageWrapper.SerializeData("chat/message", new { clientId = id, message = message }, encryptor);

            stream.Write(bytes, 0, bytes.Length);
        }

        /*
         * Sends a message to all the current connected clients.
         */
        public void Broadcast(string message)
        {
            byte[] bytes = PackageWrapper.SerializeData("chat/broadcast", new { message = message }, encryptor);

            stream.Write(bytes, 0, bytes.Length);
        }

        /*
         * Sends a new resistance to the given client.
         */
        public void SetNewResistance(string id, string resistance)
        {
            byte[] bytes = PackageWrapper.SerializeData("session/resistance", new { clientId = id, resistance = resistance }, encryptor);

            stream.Write(bytes, 0, bytes.Length);
        }

        /*
         * Starts the the session of the given client.
         */
        public void StartSession(string id)
        {
            byte[] bytes = PackageWrapper.SerializeData("session/start", new { clientId = id }, encryptor);

            stream.Write(bytes, 0, bytes.Length);
        }

        /*
         * Stops the the session of the given client.
         */
        public void StopSession(string id)
        {
            byte[] bytes = PackageWrapper.SerializeData("session/stop", new { clientId = id }, encryptor);

            stream.Write(bytes, 0, bytes.Length);
        }

        /*
         * Retrieves the data from a previous session
         */
        public void GetSession(string id)
        {
            byte[] bytes = PackageWrapper.SerializeData("doctor/clientHistory", new { clientId = id }, encryptor);

            stream.Write(bytes, 0, bytes.Length);
        }
        /*
         * Stops all the active clients
         */
        public void EmergencyStopSessions()
        {
            byte[] bytes = PackageWrapper.SerializeData("session/emergencyStop", new { }, encryptor);

            stream.Write(bytes, 0, bytes.Length);
        }
        #endregion

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
            byte[] bytes = PackageWrapper.SerializeData("doctor/login", new { username = username, password = password }, encryptor);
        
            stream.Write(bytes, 0, bytes.Length);
        }

        private void InitializeRsa()
        {
            (RSAParameters privkey, RSAParameters pubkey) keyset = encryptor.GenerateRsaKey();
            decryptor.RsaPrivateKey = keyset.privkey;

            byte[] bytes = PackageWrapper.SerializeData
            (
                "encrypt/key",
                new
                {
                    exponent = keyset.pubkey.Exponent,
                    modulus = keyset.pubkey.Modulus
                }
            );

            stream.Write(bytes, 0, bytes.Length);
        }
    }
}
