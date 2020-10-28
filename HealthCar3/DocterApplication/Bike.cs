using LiveCharts;
using LiveCharts.Wpf;
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
        public ChartValues<int> HeartRateValues { get; set; }
        public ChartValues<int> SpeedValues { get; set; }

        private int sumHeartRate = 0;
        private int heartRateCount = 0;
        private int sumSpeed = 0;
        private int speedCount = 0;

        public Bike(int bikeId, string id, string name)
        {
            BikeId = bikeId;
            ID = id;
            Name = name;

            HeartRateValues = new ChartValues<int> { 0, 0, 0, 0, 0 };
            SpeedValues = new ChartValues<int> { 0, 0, 0, 0, 0 };
        }

        public void NewHeartRate(int newHeartRate)
        {
            heartRateCount++;
            sumHeartRate += newHeartRate;

            CurrentHeartRate = newHeartRate;
            AverageHeartRate = sumHeartRate / heartRateCount;


            App.Current.Dispatcher.Invoke(delegate
            {
                HeartRateValues.Add(newHeartRate);
                if (HeartRateValues.Count > 30)
                    HeartRateValues.RemoveAt(0);
            });

        }

        public void NewSpeed(int newSpeed)
        {
            speedCount++;
            sumSpeed += newSpeed;

            CurrentSpeed = newSpeed;
            AverageSpeed = sumSpeed / speedCount;

            App.Current.Dispatcher.Invoke(delegate
            {
                SpeedValues.Add(newSpeed);
                if (SpeedValues.Count > 30)
                    SpeedValues.RemoveAt(0);
            });
        }
    }
}
