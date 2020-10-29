using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Encryption.Shared;
using Newtonsoft.Json;
using PackageUtils;

namespace Server
{
    public class Client
    {
        private readonly byte[] buffer = new byte[4];
        private readonly Decryptor decryptor;
        private readonly Encryptor encryptor;
        private readonly NetworkStream stream;
        private readonly TcpClient tcpClient;
        private readonly ClientCredentials clientCredentials;
        private string id;
        private bool keyExchanged;
        private bool loggedIn;
        private string name;
        private bool sessionActive;
        private SessionData sessionData;

        public Client(TcpClient tcpClient)
        {
            this.tcpClient = tcpClient;
            encryptor = new Encryptor();
            decryptor = new Decryptor();
            stream = this.tcpClient.GetStream();
            clientCredentials = new ClientCredentials();
            stream.BeginRead(buffer, 0, buffer.Length, OnRead, null);
        }

        public NetworkStream GetClientStream()
        {
            return stream;
        }

        public string GetId()
        {
            return id;
        }

        public bool IsSessionActive()
        {
            return sessionActive;
        }

        public void SetSession(bool active)
        {
            sessionActive = active;
        }

        public SessionData GetSessionData()
        {
            return sessionData;
        }

        public ClientCredentials GetClientCredentials()
        {
            return clientCredentials;
        }

        public bool IsLoggedIn()
        {
            return loggedIn;
        }

        public Encryptor GetEncryptor()
        {
            return encryptor;
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

        /*
         * Method that deserializes the JsonData
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
            }
            catch (Exception)
            {
                Program.Disconnect(this);
                return;
            }

            //Have to save data from client on server.
            stream.BeginRead(buffer, 0, buffer.Length, OnRead, null);
        }

        /*
         * Starts the session with this client
         */
        private void StartSession()
        {
            sessionActive = true;
            sessionData = new SessionData {ClientId = id, SessionStart = DateTime.Now, Name = name};
        }

        /*
         * Stops the session with this client
         */
        private void EndSession()
        {
            sessionActive = false;
            sessionData.SessionEnd = DateTime.Now;
            Program.SaveSession(this);
        }

        /*
         * Handles all the incoming data by looking at the tag in the received package
         */
        private void HandleData(dynamic data)
        {
            switch ((string)data.tag)
            {
                case "encrypt/key":
                    KeyEncryption(data);
                    break;
                case "chat/message":
                    SendChatMessage(data);
                    break;
                case "chat/broadcast":
                    SendBroadcastMessage(data);
                    break;
                case "client/register":
                    RegisterClient(data);
                    break;
                case "client/login":
                    ClientLogin(data);
                    break;
                case "doctor/login":
                    DoctorLogin(data);
                    break;
                case "client/update/heartRate":
                    UpdateClientHeartRate(data);
                    break;
                case "client/update/speed":
                    UpdateClientSpeed(data);
                    break;
                case "session/resistance":
                    UpdateSessionResistance(data);
                    break;
                case "session/start":
                    StartClientSession(data);
                    break;
                case "session/stop":
                    StopClientSession(data);
                    break;
                case "doctor/getSessions":
                    Program.RetrieveAllRecords();
                    break;
                case "doctor/getSessions/nextFragment":
                    Program.SendNextFragment();
                    break;
                case "session/emergencyStop":
                    Program.EmergencyStop();
                    break;
            }
        }

        #region // Response methods
        /*
         * Stops a session with the given client.
         */
        private void StopClientSession(dynamic data)
        {
            byte[] bytes;
            Client targetClient;
            Encryptor targetEncryptor = Program.GetTargetClientEncryptor((string)data.data.clientId);
            bytes = PackageWrapper.SerializeData("session/stop", new { }, targetEncryptor);
            if (!Program.ActiveSession((string)data.data.clientId, out targetClient))
                bytes = PackageWrapper.SerializeData("session/stop/error", new { message = "Client has no active session." }, encryptor);
            else
            {
                if (!Program.SendMessageToSpecificClient((string)data.data.clientId, bytes))
                    bytes = PackageWrapper.SerializeData("session/stop/error", new { message = "The Client Id could not be found." }, encryptor);
                else
                {
                    targetClient.EndSession();
                    bytes = PackageWrapper.SerializeData("session/stop/success", new { message = "Session successfully stopped." }, encryptor);
                }
            }
            tcpClient.GetStream().Write(bytes, 0, bytes.Length);
        }

        /*
         * Starts a session with the given client.
         */
        private void StartClientSession(dynamic data)
        {
            byte[] bytes;
            Client targetClient;
            Encryptor targetEncryptor = Program.GetTargetClientEncryptor((string)data.data.clientId);
            bytes = PackageWrapper.SerializeData("session/start", new { }, targetEncryptor);
            if (Program.ActiveSession((string)data.data.clientId, out targetClient))
                bytes = PackageWrapper.SerializeData("session/start/error", new { message = "Session is already active." }, encryptor);
            else
            {
                if (!Program.SendMessageToSpecificClient((string)data.data.clientId, bytes))
                    bytes = PackageWrapper.SerializeData("session/start/error", new { message = "The Client Id could not be found." }, encryptor);
                else
                {
                    targetClient.StartSession();
                    bytes = PackageWrapper.SerializeData("session/start/success", new { message = "Session successfully started." }, encryptor);
                }
            }
            tcpClient.GetStream().Write(bytes, 0, bytes.Length);
        }

        /*
         * Sends the new resistance value received from the doctor, to the given client.
         */
        private void UpdateSessionResistance(dynamic data)
        {
            byte[] bytes;
            Client targetClient;
            Encryptor targetEncryptor = Program.GetTargetClientEncryptor((string)data.data.clientId);
            string resistance = data.data.resistance;
            if (resistance == "")
                bytes = PackageWrapper.SerializeData("session/resistance/error", new { message = "Resistance value is invalid!" }, encryptor);
            else
            {
                bytes = PackageWrapper.SerializeData("session/resistance", new { resistance }, targetEncryptor);
                // Check if sending the message was successful. 
                if (!Program.SendMessageToSpecificClient((string)data.data.clientId, bytes))
                    bytes = PackageWrapper.SerializeData("session/resistance/error", new { message = "The Client Id could not be found." }, encryptor);
                else
                {
                    bytes = PackageWrapper.SerializeData("session/resistance/success", new { message = "Resistance has been updated." }, encryptor);
                    if (Program.ActiveSession((string)data.data.clientId, out targetClient))
                    {
                        targetClient.sessionData.NewResistance(float.Parse(resistance));
                    }
                }
            }
            tcpClient.GetStream().Write(bytes, 0, bytes.Length);
        }

        /*
         * Sends the new received speed from this client to the doctor application.
         */
        private void UpdateClientSpeed(dynamic data)
        {
            Encryptor targetEncryptor = Program.GetTargetClientEncryptor("0000");
            if (!sessionActive) { return; } // if we are not currently in a session do not save the data.
            if (Program.doctorClient == null) { return; }
            // Update the session with the new received speed.
            sessionData.NewSpeed((float)data.data.speed);
            // Send data to doctor client
            Program.SendMessageToSpecificClient("0000", PackageWrapper.SerializeData("client/update/speed", new { clientId = id, data.data.speed }, targetEncryptor));
        }

        /*
         * Sends the new received heart rate from this client to the doctor application.
         */
        private void UpdateClientHeartRate(dynamic data)
        {
            Encryptor targetEncryptor = Program.GetTargetClientEncryptor("0000");
            if (!sessionActive) { return; } // if we are not currently in a session do not save the data.
            if (Program.doctorClient == null) { return; }
            // Update the session with the new received heart rate.
            sessionData.NewHeartRate((int)data.data.heartRate);
            // Send data to doctor client
            Program.SendMessageToSpecificClient("0000", PackageWrapper.SerializeData("client/update/heartRate", new { clientId = id, data.data.heartRate }, targetEncryptor));
        }

        /*
         * Checks if the received username and password match the hardcoded doctor account.
         */
        private void DoctorLogin(dynamic data)
        {
            byte[] bytes;
            string username = data.data.username;
            string password = data.data.password;

            if (username == "admin" && password == "admin")
            {
                bytes = PackageWrapper.SerializeData("doctor/login/success", new { message = "Login successful" }, encryptor);
                id = "0000"; // standard doctor ID
            }
            else
                bytes = PackageWrapper.SerializeData("doctor/login/error", new { message = "The username or password is incorrect." }, encryptor);

            Program.doctorClient = this;
            tcpClient.GetStream().Write(bytes, 0, bytes.Length);
        }

        /*
         * Checks if the recieved username and password are a matching pair.
         * If so, accept the client to the server.
         */
        private void ClientLogin(dynamic data)
        {
            byte[] bytes;
            if (Program.ClientLogin((string)data.data.name, (string)data.data.password))
            {
                this.name = data.data.name;
                id = Program.registeredClients.GetValueOrDefault((this.name, (string)data.data.password), "Error: Logged in without known id!");

                Console.WriteLine($"[SERVER] Client logged in with id: {id}");
                bytes = PackageWrapper.SerializeData("client/login/success", new { message = "Login successful." }, encryptor);
                loggedIn = true;
                Program.NotifyDoctor(id, name);
            }
            else
                bytes = PackageWrapper.SerializeData("client/login/error", new { message = "The username or password is not correct." }, encryptor);

            tcpClient.GetStream().Write(bytes, 0, bytes.Length);
        }

        /*
         * Registers a new client to the server and saves the new credentials to a file.
         */
        private void RegisterClient(dynamic data)
        {
            byte[] bytes;
            string clientName = data.data.name;
            string clientPassword = data.data.password;
            if (VerifyUsername(clientName))
            {
                id = Program.GenerateId(clientName);
                name = clientName;
                Program.registeredClients.Add((clientName, (string)data.data.password), id);
                Console.WriteLine($"New Client registered with id: {id}");
                bytes = PackageWrapper.SerializeData("client/register/success", new { clientId = id, clientName, message = "Login successful." }, encryptor);
                loggedIn = true;
                clientCredentials.SetCredentials(clientName, clientPassword, id);
                Program.SaveClientData(this);
                Program.NotifyDoctor(id, name);
            }
            else
                bytes = PackageWrapper.SerializeData("client/register/error", new { message = "The username could not be set on the server." }, encryptor);

            tcpClient.GetStream().Write(bytes, 0, bytes.Length);
        }

        /*
         * Sends a chat message to all connected.
         */
        private void SendBroadcastMessage(dynamic data)
        {
            byte[] bytes;
            dynamic message = data.data.message;
            if (message == "") // if the message is empty, send an error response.
                bytes = PackageWrapper.SerializeData("chat/broadcast/error", new { message = "message is empty!" }, encryptor);
            else // otherwise just send the message.
            {
                Program.Broadcast((string)data.tag, data.data);

                bytes = PackageWrapper.SerializeData("chat/broadcast/success", new { message = "message has been received!" }, encryptor);
            }
            tcpClient.GetStream().Write(bytes, 0, bytes.Length);
        }

        /*
         * Sends a chat message to the given clientId.
         */
        private void SendChatMessage(dynamic data)
        {
            byte[] bytes;

            Encryptor targetEncryptor = Program.GetTargetClientEncryptor((string)data.data.clientId);
            dynamic message = data.data.message;
            if (message == "")
                bytes = PackageWrapper.SerializeData("chat/message/error", new { message = "message is empty!" }, encryptor);
            else
            {
                bytes = PackageWrapper.SerializeData("chat/message", new { message = (string)message }, targetEncryptor);
                // Check if sending the message was successful. 
                bytes = !Program.SendMessageToSpecificClient((string)data.data.clientId, bytes) ?
                    PackageWrapper.SerializeData("chat/message/error", new { message = "clientId is not valid!" }, encryptor) :
                    PackageWrapper.SerializeData("chat/message/success", new { message = "message has been received!" }, encryptor);
            }
            tcpClient.GetStream().Write(bytes, 0, bytes.Length);
        }

        /*
         * Generates a new key for the new connected client.
         */
        private void KeyEncryption(dynamic data)
        {
            byte[] bytes;
            byte[] exponent = data.data.exponent.ToObject<byte[]>();
            byte[] modulus = data.data.modulus.ToObject<byte[]>();

            var rsaPubKey = new RSAParameters() { Exponent = exponent, Modulus = modulus };
            (byte[] Key, byte[] iv) aesKeySet = encryptor.GenerateAesKey();

            decryptor.AesKey = aesKeySet.Key;
            decryptor.AesIv = aesKeySet.iv;

            encryptor.AesKey = aesKeySet.Key;
            encryptor.AesIv = aesKeySet.iv;

            keyExchanged = true;
            aesKeySet = encryptor.EncryptRsa(aesKeySet.Key, aesKeySet.iv, rsaPubKey);

            bytes = PackageWrapper.SerializeData
            (
                "encrypt/key/success",
                new
                {
                    key = aesKeySet.Key,
                    aesKeySet.iv
                }
            );

            tcpClient.GetStream().Write(bytes, 0, bytes.Length);
        }
        #endregion

        /*
         *  Method checks if the username consists only of letters.
         */
        private bool VerifyUsername(string name)
        {
            var validCharacters = new Regex(@"^[a-zA-Z\ ]+$");
            return validCharacters.IsMatch(name);
        }
    }
}