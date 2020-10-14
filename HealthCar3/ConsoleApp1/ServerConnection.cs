using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Security.Cryptography;
using Newtonsoft.Json;
using System.Threading;
using Newtonsoft.Json.Linq;
using PackageUtils;
using System.IO;
using Encryption.Shared;

namespace ConsoleApp1
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
        private ConnectorOption co = null;
        private Encryptor encryptor;
        private Decryptor decryptor;

        public void SetConnectorOption(ConnectorOption co) { this.co = co; }
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
            catch (Exception ex)
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
            try {
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
            //JObject jData = data as JObject;
            string tag = data.tag;
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
                case "client/register/success":
                    Console.WriteLine($"Received Id: {jData["data"].ToObject<JObject>()["clientId"].ToObject<string>()}");
                    uniqueId = jData["data"].ToObject<JObject>()["clientId"].ToObject<string>();
                    break;
                case "client/register/error":
                    Console.WriteLine($"ERROR: {jData["data"].ToObject<JObject>()["message"].ToObject<string>()}");
                    break;
                case "session/resistance":
                    float resistance = float.Parse(data.data.resistance.ToObject<string>());
                    co.WriteResistance(resistance);
                    break;
                case "session/start":
                    Console.WriteLine("Start session");
                    break;
                case "session/stop":
                    Console.WriteLine("Stop session");
                    break;
                case "chat/message":
                case "chat/broadcast":
                    string message = data.data.message.ToObject<string>();
                    Console.WriteLine($"Received Message: {message}");
                    break;
                case "chat/message/success":
                case "chat/broadcast/success":
                case "session/resistance/success":
                case "session/start/success":
                case "session/stop/success":
                    Console.WriteLine($"Succes: {jData["data"].ToObject<JObject>()["message"].ToObject<string>()}");
                    break;
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
        public void RegisterToServer(string name)
        {
            byte[] bytes = PackageWrapper.SerializeData("client/register", new { name = name, password = password }, encryptor);
            stream.Write(bytes, 0, bytes.Length);
        }

        /*
         * Method used to login to the server
         */
        public void LoginToServer(string name, string id)
        {
            byte[] bytes = PackageWrapper.SerializeData("client/login", new { name = name, password = password}, encryptor);
            stream.Write(bytes, 0, bytes.Length);
        }

        /*
         * Method used to send update messages to the server.
         */
        public void UpdateHeartRate(int heartRate)
        {
            byte[] bytes = PackageWrapper.SerializeData("client/update/heartRate", new { heartRate = heartRate}, encryptor);

            if(connected)
                stream.Write(bytes, 0, bytes.Length);
        }
        public void UpdateSpeed(float speed)
        {
            byte[] bytes = PackageWrapper.SerializeData("client/update/speed", new { speed = speed}, encryptor);

            if(connected)
                stream.Write(bytes, 0, bytes.Length);
        }

        /**
         * Initializes and sets the RSA key in the decryptor and sends the public key to the server.
         */
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
            
            stream.Write(bytes, 0 , bytes.Length);
        }
    }
}
