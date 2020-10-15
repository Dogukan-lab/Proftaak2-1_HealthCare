using System;
using System.IO;
using System.Net.Sockets;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using Encryption.Shared;
using PackageUtils;

namespace Server
{

    internal class Client
    {
        private readonly TcpClient tcpClient;
        private readonly NetworkStream stream;
        private readonly byte[] buffer = new byte[1024];
        private string id;
        private string name;
        private bool sessionActive;
        private SessionData sessionData;
        private bool loggedIn;
        private bool keyExchanged;
        private readonly Encryptor encryptor;
        private readonly Decryptor decryptor;
        public NetworkStream GetClientStream() { return stream; }
        public string GetId() { return id; }
        public bool IsSessionActive() { return sessionActive; }
        public void SetSession(bool active) { sessionActive = active; }
        public SessionData GetSessionData() { return sessionData; }
        public bool IsLoggedIn() { return loggedIn; }
        public Encryptor GetEncryptor() { return encryptor; }

        public Client(TcpClient tcpClient)
        {
            this.tcpClient = tcpClient;
            encryptor = new Encryptor();
            decryptor = new Decryptor();
            stream = this.tcpClient.GetStream();
            stream.BeginRead(buffer, 0, buffer.Length, OnRead, null);
        }
        /*
         * Method that deserializes the JsonData
         */
        private void OnRead(IAsyncResult ar)
        {
            try
            {
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
            }
            catch (IOException)
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
            sessionData = new SessionData {clientId = id, sessionStart = DateTime.Now, name = name};
        }
        /*
         * Stops the session with this client
         */
        private void EndSession()
        {
            sessionActive = false;
            sessionData.sessionEnd = DateTime.Now;
            Program.SaveSession(this);
        }

        /*
         * Handles all the incoming data by looking at the tag in the received package
         */
        private void HandleData(dynamic data)
        {
            string tag = data.tag;
            byte[] bytes;
            Client targetClient;
            Encryptor targetEncryptor;
            dynamic message;
            switch (tag)
            {
                case "encrypt/key" :
                    byte[] exponent = data.data.exponent.ToObject<byte[]>();
                    byte[] modulus = data.data.modulus.ToObject<byte[]>();
                    
                    var rsaPubKey = new RSAParameters() { Exponent = exponent, Modulus = modulus};
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
                            key = aesKeySet.Key, aesKeySet.iv
                        }
                    );
            
                    tcpClient.GetStream().Write(bytes, 0 , bytes.Length);
                    break;
                case "chat/message":
                    targetEncryptor = Program.GetTargetClientEncryptor((string)data.data.clientId);
                    message = data.data.message;
                    if (message == "")
                        bytes = PackageWrapper.SerializeData("chat/message/error", new { message = "message is empty!" }, encryptor);                    
                    else
                    {
                        bytes = PackageWrapper.SerializeData("chat/message", new { message = (string) message }, targetEncryptor);
                        // Check if sending the message was successful. 
                        bytes = !Program.SendMessageToSpecificClient((string)data.data.clientId, bytes) ? 
                            PackageWrapper.SerializeData("chat/message/error", new { message = "clientId is not valid!" }, encryptor) : 
                            PackageWrapper.SerializeData("chat/message/success", new { message = "message has been received!" }, encryptor);
                    }
                    tcpClient.GetStream().Write(bytes, 0, bytes.Length);
                    break;
                case "chat/broadcast":
                    message = data.data.message;
                    if (message == "") // if the message is empty, send an error response.
                        bytes = PackageWrapper.SerializeData("chat/broadcast/error", new { message = "message is empty!" }, encryptor);
                    else // otherwise just send the message.
                    {
                        //bytes = PackageWrapper.SerializeData("chat/broadcast", new { message = (string)data.data.message }, encryptor);
                        Program.Broadcast(tag, data.data);
                        
                        bytes = PackageWrapper.SerializeData("chat/broadcast/success", new { message = "message has been received!" }, encryptor);
                    }
                    tcpClient.GetStream().Write(bytes, 0, bytes.Length);
                    break;
                case "client/register":
                    string clientName = data.data.name;                   
                    if (CredentialVerificator.VerifyUsername(clientName))
                    {
                        id = Program.GenerateId(clientName);
                        name = clientName;
                        Program.registeredClients.Add((clientName, (string)data.data.password), id);
                        Console.WriteLine($"New Client registered with id: {id}");
                        bytes = PackageWrapper.SerializeData("client/register/success", new { clientId = id, clientName, message = "Login successful." }, encryptor);
                        loggedIn = true;
                    }
                    else
                        bytes = PackageWrapper.SerializeData("client/register/error", new { message = "The username could not be set on the server." }, encryptor);
                    tcpClient.GetStream().Write(bytes, 0, bytes.Length);
                    break;
                case "client/login":
                    if (Program.ClientLogin((string)data.data.name, (string)data.data.password))
                    {
                        this.name = data.data.name;
                        id = Program.registeredClients.GetValueOrDefault((this.name, (string)data.data.password), "Error: Logged in without known id!");
                        Console.WriteLine($"New Client logged in with id: {id}");
                        bytes = PackageWrapper.SerializeData("client/login/success", new { message = "Login successful." }, encryptor);
                        loggedIn = true;
                    }
                    else
                        bytes = PackageWrapper.SerializeData("client/login/error", new { message = "The username or password is not correct." }, encryptor);

                    tcpClient.GetStream().Write(bytes, 0, bytes.Length);
                    break;
                case "doctor/login":
                    string username = data.data.username;
                    string password = data.data.password;

                    if(username == "kees" && password == "banaan")
                    {
                        bytes = PackageWrapper.SerializeData("doctor/login/success", new { message = "Login successful" }, encryptor);
                        id = "0000"; // standard doctor ID
                    }
                    else
                        bytes = PackageWrapper.SerializeData("doctor/login/error", new { message = "The username or password is incorrect." }, encryptor);

                    tcpClient.GetStream().Write(bytes, 0, bytes.Length);
                    break;
                case "client/update/heartRate":
                    targetEncryptor = Program.GetTargetClientEncryptor("0000");
                    if (!sessionActive) { return; } // if we are not currently in a session do not save the data.
                    //Console.WriteLine($"{id}: {data.data.heartRate} BPM");
                    // Update the session with the new received heart rate.
                    sessionData.NewHeartRate((int)data.data.heartRate);
                    // Send data to doctor client
                    Program.SendMessageToSpecificClient("0000", PackageWrapper.SerializeData("client/update/heartRate", new { clientId = id, data.data.heartRate }, targetEncryptor));
                    break;
                case "client/update/speed":
                    targetEncryptor = Program.GetTargetClientEncryptor("0000");
                    if (!sessionActive) { return; } // if we are not currently in a session do not save the data.
                    //Console.WriteLine($"{id}: {data.data.speed} m/s");
                    // Update the session with the new received speed.
                    sessionData.newSpeed((float)data.data.speed);
                    // Send data to doctor client
                    Program.SendMessageToSpecificClient("0000", PackageWrapper.SerializeData("client/update/speed", new { clientId = id, data.data.speed }, targetEncryptor));
                    break;
                case "session/resistance":
                    targetEncryptor = Program.GetTargetClientEncryptor((string)data.data.clientId);
                    string resistance = data.data.resistance;
                    if (resistance == "")
                        bytes = PackageWrapper.SerializeData("session/resistance/error", new { message = "Resistance value is invalid!" }, encryptor);
                    else
                    {
                        bytes = PackageWrapper.SerializeData("session/resistance", new {resistance }, targetEncryptor);
                        // Check if sending the message was successful. 
                        if (!Program.SendMessageToSpecificClient((string)data.data.clientId, bytes))
                            bytes = PackageWrapper.SerializeData("session/resistance/error", new { message = "The Client Id could not be found." }, encryptor);
                        else
                        {
                            bytes = PackageWrapper.SerializeData("session/resistance/success", new { message = "Resistance has been updated." }, encryptor);
                            if (Program.ActiveSession((string)data.data.clientId, out targetClient)) {
                                targetClient.sessionData.NewResistance(float.Parse(resistance));
                            }
                        }
                    }
                    tcpClient.GetStream().Write(bytes, 0, bytes.Length);
                    break;
                case "session/start":
                    targetEncryptor = Program.GetTargetClientEncryptor((string)data.data.clientId);
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
                    break;
                case "session/stop":
                    targetEncryptor = Program.GetTargetClientEncryptor((string)data.data.clientId);
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
                    break;
                case "doctor/clientHistory":
                    Program.GetTargetClientEncryptor((string)data.data.clientId);
                    dynamic session = Program.GetSession((string)data.data.clientId);
                    bytes = session != null ? (byte[]) PackageWrapper.SerializeData("doctor/clientHistory/success", session, encryptor) : PackageWrapper.SerializeData("doctor/clientHistory/error", new { message = "No session found with the given ID." }, encryptor);
                    tcpClient.GetStream().Write(bytes, 0, bytes.Length);
                    break;
                case "session/emergencyStop":
                    bytes = PackageWrapper.SerializeData("session/stop", new { }, null);
                    Program.EmergencyStop(bytes);
                    break;
            }
        }
    }
}