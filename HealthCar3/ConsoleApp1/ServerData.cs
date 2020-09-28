using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;


namespace ConsoleApp1
{
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