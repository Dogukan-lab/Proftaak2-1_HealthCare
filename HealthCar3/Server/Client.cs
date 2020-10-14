using System;
using System.IO;
using System.Net.Sockets;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Security.Cryptography;
using Newtonsoft.Json.Linq;
using System.Text;
using Encryption.Shared;
using PackageUtils;

namespace Server
{

    class Client
    {
        private TcpClient tcpClient;
        private NetworkStream stream;
        private byte[] buffer = new byte[1024];
        private string id;
        private string name;
        private bool sessionActive = false;
        private SessionData sessionData = null;
        private bool loggedIn = false;
        private bool keyExchanged = false;
        private Encryptor encryptor;
        private Decryptor decryptor;
        public NetworkStream GetClientStream() { return this.stream; }
        public string GetId() { return this.id; }
        public bool IsSessionActive() { return this.sessionActive; }
        public void SetSession(bool active) { this.sessionActive = active; }
        public SessionData GetSessionData() { return this.sessionData; }
        public bool IsLoggedIn() { return this.loggedIn; }
        public Encryptor GetEncryptor() { return this.encryptor; }

        public Client(TcpClient tcpClient)
        {
            this.tcpClient = tcpClient;
            this.encryptor = new Encryptor();
            this.decryptor = new Decryptor();
            this.stream = this.tcpClient.GetStream();
            stream.BeginRead(buffer, 0, buffer.Length, new AsyncCallback(OnRead), null);
        }
        /*
         * Method that deserialises the JsonData
         */
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
                    string receivedText = System.Text.Encoding.ASCII.GetString(buffer, 0, receivedBytes);
                    receivedData = JsonConvert.DeserializeObject(receivedText);
                }
                HandleData(receivedData);
                //Console.WriteLine(receivedText);
            }
            catch (IOException)
            {
                Program.Disconnect(this);
                return;
            }

            //TODO 
            //Have to save data from client on server.

            stream.BeginRead(buffer, 0, buffer.Length, new AsyncCallback(OnRead), null);
        }

        /*
         * Starts the session with this client
         */
        public void StartSession()
        {
            sessionActive = true;
            sessionData = new SessionData();
            sessionData.clientId = id;
            sessionData.sessionStart = DateTime.Now;
            sessionData.name = name;
        }
        /*
         * Stops the session with this client
         */
        public void EndSession()
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
            byte[] bytes = new byte[0];
            string message = "";
            Client targetClient = null;
            Encryptor targetEncryptor = null;
            switch (tag)
            {
                case "encrypt/key" :
                    byte[] exponent = data.data.exponent.ToObject<byte[]>();
                    byte[] modulus = data.data.modulus.ToObject<byte[]>();
                    
                    RSAParameters rsaPubkey = new RSAParameters() { Exponent = exponent, Modulus = modulus};
                    (byte[] Key, byte[] iv) aesKeyset = encryptor.GenerateAesKey();

                    decryptor.AesKey = aesKeyset.Key;
                    decryptor.AesIv = aesKeyset.iv;

                    encryptor.AesKey = aesKeyset.Key;
                    encryptor.AesIv = aesKeyset.iv;

                    keyExchanged = true;
                    aesKeyset = encryptor.EncryptRsa(aesKeyset.Key, aesKeyset.iv, rsaPubkey);
                    
                    bytes = PackageWrapper.SerializeData
                    (
                        "encrypt/key/success",
                        new
                        {
                            key = aesKeyset.Key,
                            iv = aesKeyset.iv
                        }
                    );
            
                    stream.Write(bytes, 0 , bytes.Length);
                    break;
                case "chat/message":
                    targetEncryptor = Program.GetTargetClientEncryptor((string)data.data.clientId);
                    message = data.data.message;
                    if (message == "")
                        bytes = PackageWrapper.SerializeData("chat/message/error", new { message = "message is empty!" });                    
                    else
                    {
                        bytes = PackageWrapper.SerializeData("chat/message", new { message = message }, targetEncryptor);
                        // Check if sending the message was successful. 
                        if (!Program.SendMessageToSpecificClient((string)data.data.clientId, bytes))
                            bytes = PackageWrapper.SerializeData("chat/message/error", new { message = "clientId is not valid!" }, encryptor);
                        else
                            bytes = PackageWrapper.SerializeData("chat/message/success", new { message = "message had been received!" });
                    }
                    stream.Write(bytes, 0, bytes.Length);
                    break;
                case "chat/broadcast":
                    message = data.data.message;
                    if (message == "") // if the message is empty, send an error response.
                        bytes = PackageWrapper.SerializeData("chat/broadcast/error", new { message = "message is empty!" });
                    else // otherwise just send the message.
                    {
                        //bytes = PackageWrapper.SerializeData("chat/broadcast", new { message = (string)data.data.message }, encryptor);
                        Program.Broadcast(tag, data.data);
                        
                        bytes = PackageWrapper.SerializeData("chat/broadcast/success", new { message = "message has been received!" });
                    }
                    stream.Write(bytes, 0, bytes.Length);
                    break;
                case "client/register":
                    string name = data.data.name;                   
                    if (CredentialVarificator.VerifyUsername(name))
                    {
                        id = Program.GenerateId(name);
                        this.name = name;
                        Program.registeredClients.Add((name, (string)data.data.password), id);
                        Console.WriteLine($"New Client registered with id: {id}");
                        bytes = PackageWrapper.SerializeData("client/register/success", new { clientId = id, clientName = name, message = "Login successful." }, encryptor);
                        loggedIn = true;
                    }
                    else
                        bytes = PackageWrapper.SerializeData("client/register/error", new { message = "The username could not be set on the server." }, encryptor);
                    stream.Write(bytes, 0, bytes.Length);
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

                    stream.Write(bytes, 0, bytes.Length);
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

                    stream.Write(bytes, 0, bytes.Length);
                    break;
                case "client/update/heartRate":
                    targetEncryptor = Program.GetTargetClientEncryptor("0000");
                    if (!sessionActive) { return; } // if we are not currently in a session do not save the data.
                    //Console.WriteLine($"{id}: {data.data.heartRate} BPM");
                    // Update the session with the new received heart rate.
                    sessionData.newHeartRate((int)data.data.heartRate);
                    // Send data to doctor client
                    Program.SendMessageToSpecificClient("0000", PackageWrapper.SerializeData("client/update/heartRate", new { clientId = id, heartRate = data.data.heartRate }, targetEncryptor));
                    break;
                case "client/update/speed":
                    targetEncryptor = Program.GetTargetClientEncryptor("0000");
                    if (!sessionActive) { return; } // if we are not currently in a session do not save the data.
                    //Console.WriteLine($"{id}: {data.data.speed} m/s");
                    // Update the session with the new received speed.
                    sessionData.newSpeed((float)data.data.speed);
                    // Send data to doctor client
                    Program.SendMessageToSpecificClient("0000", PackageWrapper.SerializeData("client/update/speed", new { clientId = id, speed = data.data.speed }, targetEncryptor));
                    break;
                case "session/resistance":
                    targetEncryptor = Program.GetTargetClientEncryptor((string)data.data.clientId);
                    string resistance = data.data.resistance;
                    if (resistance == "")
                        bytes = PackageWrapper.SerializeData("session/resistance/error", new { message = "Resistance value is invalid!" });
                    else
                    {
                        bytes = PackageWrapper.SerializeData("session/resistance", new { resistance = resistance }, targetEncryptor);
                        // Check if sending the message was successful. 
                        if (!Program.SendMessageToSpecificClient((string)data.data.clientId, bytes))
                            bytes = PackageWrapper.SerializeData("session/resistance/error", new { message = "The Client Id could not be found." }, encryptor);
                        else
                        {
                            bytes = PackageWrapper.SerializeData("session/resistance/success", new { message = "Resistance has been updated." }, encryptor);
                            if (Program.ActiveSession((string)data.data.clientId, out targetClient)) {
                                targetClient.sessionData.newResistance(float.Parse(resistance));
                                targetClient = null;
                            }
                        }

                    }
                    stream.Write(bytes, 0, bytes.Length);
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
                            targetClient = null;
                            bytes = PackageWrapper.SerializeData("session/start/success", new { message = "Session successfully started." }, encryptor);
                        }
                    }
                    stream.Write(bytes, 0, bytes.Length);
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
                            targetClient = null;
                            bytes = PackageWrapper.SerializeData("session/stop/success", new { message = "Session successfully stopped." }, encryptor);
                        }
                    }
                    stream.Write(bytes, 0, bytes.Length);
                    break;
                case "doctor/clientHistory":
                    targetEncryptor = Program.GetTargetClientEncryptor((string)data.data.clientId);
                    dynamic session = Program.GetSession((string)data.data.clientId);
                    if (session != null)
                        bytes = PackageWrapper.SerializeData("doctor/clientHistory/success", session, encryptor);
                    else
                        bytes = PackageWrapper.SerializeData("doctor/clientHistory/error", new { message = "No session found with the given ID." }, encryptor);
                    stream.Write(bytes, 0, bytes.Length);
                    break;
                case "session/emergencyStop":
                    bytes = PackageWrapper.SerializeData("session/stop", new { }, targetEncryptor);
                    Program.EmergencyStop(bytes);
                    break;
            }
        }
    }
}