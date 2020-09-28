using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;

namespace ConsoleApp1
{
    class ServerConnection
    {
        private TcpClient clientConnection;
        private NetworkStream stream;
        private String IPAddress;
        private int port;
        private int totalTries;
        private readonly int MAXRECONTRIES = 3;
        public ServerConnection(String IPAddress, int port)
        {
            clientConnection = new TcpClient();

            OnConnection(IPAddress, port);
        }

        /*
         * Method used to make a connection with the server.
         */
        public void OnConnection(string IPAddress, int port)
        {
            try
            {
                clientConnection.Connect(IPAddress, port);
                stream = clientConnection.GetStream();
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex);
                if (!clientConnection.Connected && totalTries < 3)
                {
                    OnConnection(IPAddress, port);
                }
                else
                {
                    OnDisconnect();
                }
            }
        }

        /*
         * Method used to disconnect from the server.
         */
        public void OnDisconnect()
        {
            stream.Dispose();
            clientConnection.Close();
        }

        /*
         * Method used to send messages to the server.
         */
        public void Message(string jsonData)
        {

            var msg = Encoding.ASCII.GetBytes(jsonData);
            stream.Write(msg, 0, msg.Length);
        }
    }
}
