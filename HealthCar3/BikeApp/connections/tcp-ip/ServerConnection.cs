﻿using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using BikeApp.connections.bluetooth;
using Encryption.Shared;
using Newtonsoft.Json;
using PackageUtils;

namespace BikeApp.connections
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
        private ConnectorOption co;
        private bool connected;
        private bool keyExchanged;
        private bool loggedIn;
        private NetworkStream stream;
        private int totalTries;
        private readonly VpnConnector vpnConnector;

        public ServerConnection()
        {
            clientConnection = new TcpClient();
            encryptor = new Encryptor();
            decryptor = new Decryptor();
            vpnConnector = new VpnConnector(new JsonSerializerSettings());
            Connect(IpAddress, Port);
        }

        public void SetConnectorOption(ConnectorOption connector)
        {
            co = connector;
            vpnConnector.SetConnectorOptions(co);
        }
        public bool IsLoggedIn()
        {
            return loggedIn;
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
            string tag = data.tag;
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
                case "client/register/success":
                case "client/login/success": // Succesfully logged in or registered to the server.
                    Console.WriteLine(data.data.message);
                    loggedIn = true;
                    break;
                case "client/register/error":
                case "client/login/error": // Lets the client know that the input credentials were incorrect or was unable to register.
                    Console.WriteLine($@"ERROR: {data.data.message}");
                    break;
                case "session/resistance": // Sets the new received resistance from the doctor.
                    float resistance = float.Parse(data.data.resistance.ToObject<string>());
                    co.WriteResistance(resistance);
                    break;
                case "session/start": // Starts the session, by starting the vr.
                    Console.WriteLine(@"Start session");
                    vpnConnector.Connect();
                    vpnConnector.CommandCenter.Running = true;
                    break;
                case "session/stop": // Stops the session, by stopping the vr.
                    Console.WriteLine(@"Stop session");
                    vpnConnector.CommandCenter.ResetScene();
                    vpnConnector.CommandCenter.Running = false;
                    vpnConnector.Disconnect();
                    break;
                case "chat/message":
                case "chat/broadcast": // Displays the new received message to the client.
                    string message = data.data.message.ToObject<string>();
                    Console.WriteLine($@"Received Message: {message}");
                    vpnConnector.CommandCenter.ChatMsg = message;
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
         * Method used to register to the server
         */
        public void RegisterToServer(string name, string password)
        {
            var bytes = PackageWrapper.SerializeData("client/register", new {name, password}, encryptor);
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
         * Sends the new heart rate value to the server.
         */
        public void UpdateHeartRate(int heartRate)
        {
            var bytes = PackageWrapper.SerializeData("client/update/heartRate", new {heartRate}, encryptor);

            if (connected)
                stream.Write(bytes, 0, bytes.Length);
        }
        /*
         * Sends the new speed value to the server.
         */
        public void UpdateSpeed(float speed)
        {
            var bytes = PackageWrapper.SerializeData("client/update/speed", new {speed}, encryptor);

            if (connected)
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

            stream.Write(bytes, 0, bytes.Length);
        }
    }
}