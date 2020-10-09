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
        private bool sessionActive = false;
        private bool keyExchanged = false;
        private Encryptor encryptor;
        private Decryptor decryptor;
        
        public NetworkStream GetClientStream() { return this.stream; }
        public string GetId() { return this.id; }
        public bool IsSessionActive() { return this.sessionActive; }
        public void SetSession(bool active) { this.sessionActive = active; }


        public Client(TcpClient tcpClient)
        {
            this.tcpClient = tcpClient;
            this.encryptor = new Encryptor();
            this.decryptor = new Decryptor();

            this.stream = this.tcpClient.GetStream();
            
            stream.BeginRead(buffer, 0, buffer.Length, new AsyncCallback(OnRead), null);
        }
        /*
         * Method that deserilises the JsonData
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
            //Have to save data from client on server..

            stream.BeginRead(buffer, 0, buffer.Length, new AsyncCallback(OnRead), null);
        }
        /*
         * Handles all the incoming data by looking at the tag in the received package
         */
        private void HandleData(dynamic data)
        {
            JObject jData = data as JObject;
            string tag = jData["tag"].ToObject<string>();
            byte[] bytes = new byte[0];
            string message = "";
            Client targetClient = null;
            switch (tag)
            {
                case "encrypt/key" :
                    byte[] exponent = jData["data"].ToObject<JObject>()["exponent"].ToObject<byte[]>();
                    byte[] modulus = jData["data"].ToObject<JObject>()["modulus"].ToObject<byte[]>();
                    
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
                    message = jData["data"].ToObject<JObject>()["message"].ToObject<string>();
                    if (message == "")
                        bytes = PackageWrapper.SerializeData("chat/message/error", new { message = "message is empty!" }, encryptor);                    
                    else
                    {
                        bytes = PackageWrapper.SerializeData("chat/message", new { message = message }, encryptor);
                        // Check if sending the message was successful. 
                        if (!Program.SendMessageToSpecificClient(jData["data"].ToObject<JObject>()["clientId"].ToObject<string>(), bytes))
                            bytes = PackageWrapper.SerializeData("chat/message/error", new { message = "clientId is not valid!" }, encryptor);
                        else
                            bytes = PackageWrapper.SerializeData("chat/message/success", new { message = "message had been received!" }, encryptor);
                    }
                    stream.Write(bytes, 0, bytes.Length);
                    break;
                case "chat/broadcast":
                    message = jData["data"].ToObject<JObject>()["message"].ToObject<string>();
                    if (message == "") // if the message is empty, send an error response.
                        bytes = PackageWrapper.SerializeData("chat/broadcast/error", new { message = "message is empty!" }, encryptor);
                    else // otherwise just send the message.
                    {
                        bytes = PackageWrapper.SerializeData("chat/broadcast", new { message = jData["data"].ToObject<JObject>()["message"].ToObject<string>() }, encryptor);
                        Program.Broadcast(bytes);
                        
                        bytes = PackageWrapper.SerializeData("chat/broadcast/success", new { message = "message has been received!" }, encryptor);
                    }
                    stream.Write(bytes, 0, bytes.Length);
                    break;
                case "client/register":
                    string name = jData["data"].ToObject<JObject>()["name"].ToObject<string>();                   
                    if (CredentialVarificator.VerifyUserName(name))
                    {
                        id = Program.GenerateId(name);
                        bytes = PackageWrapper.SerializeData("client/register/success", new { clientId = id, clientName = name }, encryptor);
                    }
                    else
                        bytes = PackageWrapper.SerializeData("client/register/error", new { message = "The username could not be set on the server." }, encryptor);
                    stream.Write(bytes, 0, bytes.Length);
                    break;
                case "client/login":
                    break;
                case "client/update/heartRate":
                    Console.WriteLine($"{id}: {jData["data"].ToObject<JObject>()["heartRate"].ToObject<string>()} BPM");
                    break;
                case "client/update/speed":
                    Console.WriteLine($"{id}: {jData["data"].ToObject<JObject>()["speed"].ToObject<string>()} m/s");
                    break;
                case "session/resistance":
                    string resistance = jData["data"].ToObject<JObject>()["resistance"].ToObject<string>();
                    if (resistance == "")
                        bytes = PackageWrapper.SerializeData("session/resistance/error", new { message = "Resistance value is invalid!" }, encryptor);
                    else
                    {
                        bytes = PackageWrapper.SerializeData("session/resistance", new { resistance = resistance }, encryptor);
                        // Check if sending the message was successful. 
                        if (!Program.SendMessageToSpecificClient(jData["data"].ToObject<JObject>()["clientId"].ToObject<string>(), bytes))
                            bytes = PackageWrapper.SerializeData("session/resistance/error", new { message = "The Client Id could not be found." }, encryptor);
                        else
                            bytes = PackageWrapper.SerializeData("session/resistance/success", new { message = "Resistance has been updated." }, encryptor);
                    }
                    stream.Write(bytes, 0, bytes.Length);
                    break;
                case "session/start":
                    bytes = PackageWrapper.SerializeData("session/start", new { }, encryptor);
                    if (Program.ActiveSession(jData["data"].ToObject<JObject>()["clientId"].ToObject<string>(), out targetClient))
                        bytes = PackageWrapper.SerializeData("session/start/error", new { message = "Session is already active." }, encryptor);
                    else
                    {
                        if (!Program.SendMessageToSpecificClient(jData["data"].ToObject<JObject>()["clientId"].ToObject<string>(), bytes))
                            bytes = PackageWrapper.SerializeData("session/start/error", new { message = "The Client Id could not be found." }, encryptor);
                        else
                        {
                            targetClient.SetSession(true);
                            targetClient = null;
                            bytes = PackageWrapper.SerializeData("session/start/success", new { message = "Session successfully started." }, encryptor);
                        }
                    }
                    stream.Write(bytes, 0, bytes.Length);
                    break;
                case "session/stop":
                    bytes = PackageWrapper.SerializeData("session/stop", new { }, encryptor);
                    if (!Program.ActiveSession(jData["data"].ToObject<JObject>()["clientId"].ToObject<string>(), out targetClient))
                        bytes = PackageWrapper.SerializeData("session/stop/error", new { message = "Client has no active session." }, encryptor);
                    else
                    {
                        if (!Program.SendMessageToSpecificClient(jData["data"].ToObject<JObject>()["clientId"].ToObject<string>(), bytes))
                            bytes = PackageWrapper.SerializeData("session/stop/error", new { message = "The Client Id could not be found." }, encryptor);
                        else
                        {
                            targetClient.SetSession(false);
                            targetClient = null;
                            bytes = PackageWrapper.SerializeData("session/stop/success", new { message = "Session successfully stopped." }, encryptor);
                        }
                    }
                    stream.Write(bytes, 0, bytes.Length);
                    break;
            }
        }
    }
}