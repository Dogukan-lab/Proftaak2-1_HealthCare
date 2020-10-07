using System;
using System.IO;
using System.Net.Sockets;
using Newtonsoft.Json;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using System.Text;
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
        private SessionData sessionData = null;
        public NetworkStream GetClientStream() { return this.stream; }
        public string GetId() { return this.id; }
        public bool IsSessionActive() { return this.sessionActive; }
        public void SetSession(bool active) { this.sessionActive = active; }
        public SessionData GetSessionData() { return this.sessionData; }

        public Client(TcpClient tcpClient)
        {
            this.tcpClient = tcpClient;
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
                var receivedText = System.Text.Encoding.ASCII.GetString(buffer, 0, receivedBytes);
                dynamic receivedData = JsonConvert.DeserializeObject(receivedText);
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
                case "chat/message":
                    message = jData["data"].ToObject<JObject>()["message"].ToObject<string>();
                    if (message == "")
                        bytes = PackageWrapper.SerializeData("chat/message/error", new { message = "message is empty!" });                    
                    else
                    {
                        bytes = PackageWrapper.SerializeData("chat/message", new { message = message });
                        // Check if sending the message was successful. 
                        if (!Program.SendMessageToSpecificClient(jData["data"].ToObject<JObject>()["clientId"].ToObject<string>(), bytes))
                            bytes = PackageWrapper.SerializeData("chat/message/error", new { message = "clientId is not valid!" });
                        else
                            bytes = PackageWrapper.SerializeData("chat/message/success", new { message = "message had been received!" });
                    }
                    stream.Write(bytes, 0, bytes.Length);
                    break;
                case "chat/broadcast":
                    message = jData["data"].ToObject<JObject>()["message"].ToObject<string>();
                    if (message == "") // if the message is empty, send an error response.
                        bytes = PackageWrapper.SerializeData("chat/broadcast/error", new { message = "message is empty!" });
                    else // otherwise just send the message.
                    {
                        bytes = PackageWrapper.SerializeData("chat/broadcast", new { message = jData["data"].ToObject<JObject>()["message"].ToObject<string>() });
                        Program.Broadcast(bytes);
                        
                        bytes = PackageWrapper.SerializeData("chat/broadcast/success", new { message = "message has been received!" });
                    }
                    stream.Write(bytes, 0, bytes.Length);
                    break;
                case "client/register":
                    string name = jData["data"].ToObject<JObject>()["name"].ToObject<string>();                   
                    if (CredentialVarificator.VerifyUserName(name))
                    {
                        id = Program.GenerateId(name);
                        bytes = PackageWrapper.SerializeData("client/register/success", new { clientId = id, clientName = name });
                    }
                    else
                        bytes = PackageWrapper.SerializeData("client/register/error", new { message = "The username could not be set on the server." });

                    stream.Write(bytes, 0, bytes.Length);
                    break;
                case "client/login":
                    break;
                case "doctor/login":
                    string username = jData["data"].ToObject<JObject>()["username"].ToObject<string>();
                    string password = jData["data"].ToObject<JObject>()["password"].ToObject<string>();

                    if(username == "kees" && password == "banaan")
                    {
                        bytes = PackageWrapper.SerializeData("doctor/login/success", new { message = "Login successful" });
                        id = "0000"; // standard doctor ID
                    }
                    else
                        bytes = PackageWrapper.SerializeData("doctor/login/error", new { message = "The username or password is incorrect." });

                    stream.Write(bytes, 0, bytes.Length);
                    break;
                case "client/update/heartRate":
                    if(!sessionActive) { return; } // if we are not currently in a session do not save the data.
                    //Console.WriteLine($"{id}: {jData["data"].ToObject<JObject>()["heartRate"].ToObject<string>()} BPM");
                    // Update the session with the new received heart rate.
                    sessionData.newHeartRate(jData["data"].ToObject<JObject>()["heartRate"].ToObject<int>());
                    // Send data to doctor client
                    Program.SendMessageToSpecificClient("0000", PackageWrapper.SerializeData("client/update/heartRate", new { clientId = id, heartRate = jData["data"].ToObject<JObject>()["heartRate"].ToObject<string>() }));
                    break;
                case "client/update/speed":
                    if (!sessionActive) { return; } // if we are not currently in a session do not save the data.
                    //Console.WriteLine($"{id}: {jData["data"].ToObject<JObject>()["speed"].ToObject<string>()} m/s");
                    // Update the session with the new received speed.
                    float newSpeed = jData["data"].ToObject<JObject>()["speed"].ToObject<float>();
                    sessionData.newSpeed(newSpeed);
                    // Send data to doctor client
                    Program.SendMessageToSpecificClient("0000", PackageWrapper.SerializeData("client/update/speed", new { clientId = id, speed = jData["data"].ToObject<JObject>()["speed"].ToObject<string>() }));
                    break;
                case "session/resistance":
                    string resistance = jData["data"].ToObject<JObject>()["resistance"].ToObject<string>();
                    if (resistance == "")
                        bytes = PackageWrapper.SerializeData("session/resistance/error", new { message = "Resistance value is invalid!" });
                    else
                    {
                        bytes = PackageWrapper.SerializeData("session/resistance", new { resistance = resistance });
                        // Check if sending the message was successful. 
                        if (!Program.SendMessageToSpecificClient(jData["data"].ToObject<JObject>()["clientId"].ToObject<string>(), bytes))
                            bytes = PackageWrapper.SerializeData("session/resistance/error", new { message = "The Client Id could not be found." });
                        else
                            bytes = PackageWrapper.SerializeData("session/resistance/success", new { message = "Resistance has been updated." });
                    }
                    stream.Write(bytes, 0, bytes.Length);
                    break;
                case "session/start":
                    bytes = PackageWrapper.SerializeData("session/start", new { });
                    if (Program.ActiveSession(jData["data"].ToObject<JObject>()["clientId"].ToObject<string>(), out targetClient))
                        bytes = PackageWrapper.SerializeData("session/start/error", new { message = "Session is already active." });
                    else
                    {
                        if (!Program.SendMessageToSpecificClient(jData["data"].ToObject<JObject>()["clientId"].ToObject<string>(), bytes))
                            bytes = PackageWrapper.SerializeData("session/start/error", new { message = "The Client Id could not be found." });
                        else
                        {
                            targetClient.SetSession(true);
                            bytes = PackageWrapper.SerializeData("session/start/success", new { message = "Session successfully started." });
                            targetClient.sessionData = new SessionData();
                            targetClient.sessionData.clientId = targetClient.GetId();
                            targetClient = null;
                        }
                    }
                    stream.Write(bytes, 0, bytes.Length);
                    break;
                case "session/stop":
                    bytes = PackageWrapper.SerializeData("session/stop", new { });
                    if (!Program.ActiveSession(jData["data"].ToObject<JObject>()["clientId"].ToObject<string>(), out targetClient))
                        bytes = PackageWrapper.SerializeData("session/stop/error", new { message = "Client has no active session." });
                    else
                    {
                        if (!Program.SendMessageToSpecificClient(jData["data"].ToObject<JObject>()["clientId"].ToObject<string>(), bytes))
                            bytes = PackageWrapper.SerializeData("session/stop/error", new { message = "The Client Id could not be found." });
                        else
                        {
                            targetClient.SetSession(false);
                            bytes = PackageWrapper.SerializeData("session/stop/success", new { message = "Session successfully stopped." });
                            Program.SaveSession(targetClient);
                            targetClient = null;
                        }
                    }
                    stream.Write(bytes, 0, bytes.Length);
                    break;
                case "doctor/clientHistory":
                    dynamic session = Program.GetSession(jData["data"].ToObject<JObject>()["clientId"].ToObject<string>());
                    if (session != null)
                        bytes = PackageWrapper.SerializeData("doctor/clientHistory/success", session);
                    else
                        bytes = PackageWrapper.SerializeData("doctor/clientHistory/error", new { message = "No session found with the given ID." });
                    stream.Write(bytes, 0, bytes.Length);
                    break;
            }
        }
    }
}