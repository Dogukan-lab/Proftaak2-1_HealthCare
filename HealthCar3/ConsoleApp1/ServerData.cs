using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;


namespace ConsoleApp1
{
    /*
     * Class used to save simulation data as an object to be sent to the server.
     */
    class ServerData
    {
        public int heartRate;
        public float speed;

        public ServerData(int heartRate, float speed)
        {
            this.heartRate = heartRate;
            this.speed = speed;

        }
    }
}