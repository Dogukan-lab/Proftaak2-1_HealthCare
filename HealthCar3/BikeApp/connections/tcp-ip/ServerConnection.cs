using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using BikeApp.connections.bluetooth;
using Encryption.Shared;
using Newtonsoft.Json;
using PackageUtils;

namespace BikeApp.connections
{
    internal class ServerConnection
    {
        private readonly TcpClient clientConnection;
        private NetworkStream stream;
        private const string IpAddress = "127.0.0.1";
        private const int Port = 1330;
        private int totalTries;
        private const int MaxReconTries = 3;
        private readonly byte[] buffer = new byte[1024];
        private bool connected;
        private bool loggedIn;
        private bool keyExchanged;
        private ConnectorOption co;
        private readonly Encryptor encryptor;
        private readonly Decryptor decryptor;

        public void SetConnectorOption(ConnectorOption connector) { co = connector; }
        public bool IsLoggedIn() { return loggedIn; }
        public ServerConnection()
        {
            clientConnection = new TcpClient();
            encryptor = new Encryptor();
            decryptor = new Decryptor();
            Connect(IpAddress, Port);
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

                if (clientConnection.Connected)
                {
                    OnConnected();
                }
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

        private void OnRead(IAsyncResult ar)
        {
            try {
                var receivedBytes = stream.EndRead(ar);
                dynamic receivedData;

                if (keyExchanged)
                {
                    receivedData = JsonConvert.DeserializeObject(decryptor.DecryptAes(buffer, 0, receivedBytes));
                }
                else
                {
                    var receivedText = Encoding.ASCII.GetString(buffer, 0, receivedBytes);
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

        private void HandleData(dynamic data)
        {
            string tag = data.tag;
            switch (tag)
            {
                case "encrypt/key/success":
                    byte[] key = decryptor.DecryptRsa(data.data.key.ToObject<byte[]>());
                    byte[] iv = decryptor.DecryptRsa(data.data.iv.ToObject<byte[]>());
                    
                    encryptor.AesKey = key;
                    encryptor.AesIv = iv;

                    decryptor.AesKey = key;
                    decryptor.AesIv = iv;

                    keyExchanged = true;
                    break;
                case "client/register/success":
                case "client/login/success":
                    Console.WriteLine(data.data.message);
                    loggedIn = true;
                    break;
                case "client/register/error":
                case "client/login/error":
                    Console.WriteLine($@"ERROR: {data.data.message}");
                    break;
                case "session/resistance":
                    float resistance = float.Parse(data.data.resistance.ToObject<string>());
                    co.WriteResistance(resistance);
                    break;
                case "session/start":
                    Console.WriteLine(@"Start session");
                    break;
                case "session/stop":
                    Console.WriteLine(@"Stop session");
                    break;
                case "chat/message":
                case "chat/broadcast":
                    string message = data.data.message.ToObject<string>();
                    Console.WriteLine($@"Received Message: {message}");
                    break;
            }
        }

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
         * Method used to register to the server
         */
        public void RegisterToServer(string name, string password)
        {
            var bytes = PackageWrapper.SerializeData("client/register", new {name, password }, encryptor);
            stream.Write(bytes, 0, bytes.Length);
        }

        /*
         * Method used to login to the server
         */
        public void LoginToServer(string name, string password)
        {
            var bytes = PackageWrapper.SerializeData("client/login", new {name, password}, encryptor);
            stream.Write(bytes, 0, bytes.Length);
        }

        /*
         * Method used to send update messages to the server.
         */
        public void UpdateHeartRate(int heartRate)
        {
            var bytes = PackageWrapper.SerializeData("client/update/heartRate", new {heartRate}, encryptor);

            if(connected)
                stream.Write(bytes, 0, bytes.Length);
        }
        
        public void UpdateSpeed(float speed)
        {
            var bytes = PackageWrapper.SerializeData("client/update/speed", new {speed}, encryptor);

            if(connected)
                stream.Write(bytes, 0, bytes.Length);
        }

        /**
         * Initializes and sets the RSA key in the decryptor and sends the public key to the server.
         */
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
            
            stream.Write(bytes, 0 , bytes.Length);
        }
    }
}
