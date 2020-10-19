using System;
using System.Collections.Generic;
using System.Text;

namespace DocterApplication
{
    class Bike
    {
        public int BikeId { get; set; }
        public string ID { get; set; }
        public string Name { get; set; }
        public int CurrentHeartRate { get; set; }
        public int AverageHeartRate { get; set; }
        public int CurrentSpeed { get; set; }
        public int AverageSpeed { get; set; }
        public int Resistance { get; set; }
        public bool ActiveSession { get; set; }

        private int sumHeartRate = 0;
        private int heartRateCount = 0;
        private int sumSpeed = 0;
        private int speedCount = 0;

        public Bike(int bikeId, string id, string name)
        {
            BikeId = bikeId;
            ID = id;
            Name = name;
        }

        public void NewHeartRate(int newHeartRate)
        {
            heartRateCount++;
            sumHeartRate += newHeartRate;

            CurrentHeartRate = newHeartRate;
            AverageHeartRate = sumHeartRate / heartRateCount;
        }

        public void NewSpeed(int newSpeed)
        {
            speedCount++;
            sumSpeed += newSpeed;

            CurrentSpeed = newSpeed;
            AverageSpeed = sumSpeed / speedCount;
        }     
    }
}
