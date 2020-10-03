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
        public NetworkStream GetClientStream() { return this.stream; }
        public string GetId() { return this.id; }


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
                Console.WriteLine(receivedText);
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
                            bytes = PackageWrapper.SerializeData("chat/message/error", new { message = "destinationId is not valid!" });
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
                case "update/heartrate":
                    break;
                case "update/speed":
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
            }
        }
    }
}