using System;
using System.IO;
using System.Net.Sockets;

namespace Server
{

    class Client
    {
        public int clientID { get; set; }
        private TcpClient tcpClient;
        private NetworkStream stream;
        private byte[] buffer = new byte[1024];
        private string totalBuffer = "";

        public Client(int clientID,TcpClient tcpClient)
        {
            this.clientID = clientID;
            this.tcpClient = tcpClient;

            this.stream = this.tcpClient.GetStream();
            stream.BeginRead(buffer, 0, buffer.Length, new AsyncCallback(OnRead), null);
        }

        private void OnRead(IAsyncResult ar)
        {
            try
            {
                int receivedBytes = stream.EndRead(ar);
                string receivedText = System.Text.Encoding.ASCII.GetString(buffer, 0, receivedBytes);
                totalBuffer += receivedText;
            }
            catch (IOException)
            {
                Program.Disconnect(this);
                return;
            }

            //TODO 
            //Read and print out the data from the message.
            //Have to save data from client on server.
            /*while (totalBuffer.Contains("\r\n\r\n"))
            {
                string packet = totalBuffer.Substring(0, totalBuffer.IndexOf("\r\n\r\n"));
                totalBuffer = totalBuffer.Substring(totalBuffer.IndexOf("\r\n\r\n") + 4);
                string[] packetData = Regex.Split(packet, "\r\n");
                handleData(packetData);
            }*/
            stream.BeginRead(buffer, 0, buffer.Length, new AsyncCallback(OnRead), null);
        }

        private void handleData(string[] packetData)
        {
            Console.WriteLine($"Got a packet: {packetData[0]}");

            //TODO 
            //Make a method to handle the incoming data.

           /* switch (packetData[0])
            {
                case "login":
                    if (!assertPacketData(packetData, 3))
                        return;
                    if (packetData[1] == packetData[2])
                    {
                        Write("login\r\nok");
                        this.UserName = packetData[1];
                    }
                    else
                        Write("login\r\nerror, wrong password");
                    break;
                case "chat":
                    {
                        if (!assertPacketData(packetData, 2))
                            return;
                        string message = $"{this.UserName} : {packetData[1]}";
                        Program.Broadcast($"chat\r\n{message}");
                        break;
                    }
                case "pm":
                    {
                        if (!assertPacketData(packetData, 3))
                            return;
                        string message = $"{this.UserName} : {packetData[2]}";
                        Program.SendToUser(packetData[1], message);
                        break;
                    }
            }*/


        }

 /*       private bool assertPacketData(string[] packetData, int requiredLength)
        {
            if (packetData.Length < requiredLength)
            {
                Write("error");
                return false;
            }
            return true;
        }*/

/*        public void Write(string data)
        {
            var dataAsBytes = System.Text.Encoding.ASCII.GetBytes(data + "\r\n\r\n");
            stream.Write(dataAsBytes, 0, dataAsBytes.Length);
            stream.Flush();
        }*/
    }
}