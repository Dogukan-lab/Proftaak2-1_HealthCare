using System.Windows;
using LiveCharts;

namespace DocterApplication
{
    public class Bike
    {
        private int heartRateCount;
        private int speedCount;

        private int sumHeartRate;
        private int sumSpeed;

        public Bike(int bikeId, string id, string name)
        {
            BikeId = bikeId;
            ID = id;
            Name = name;

            HeartRateValues = new ChartValues<int> {0, 0, 0, 0, 0};
            SpeedValues = new ChartValues<int> {0, 0, 0, 0, 0};
        }

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

        public void NewHeartRate(int newHeartRate)
        {
            heartRateCount++;
            sumHeartRate += newHeartRate;

            CurrentHeartRate = newHeartRate;
            AverageHeartRate = sumHeartRate / heartRateCount;


            Application.Current.Dispatcher.Invoke(delegate
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

            Application.Current.Dispatcher.Invoke(delegate
            {
                SpeedValues.Add(newSpeed);
                if (SpeedValues.Count > 30)
                    SpeedValues.RemoveAt(0);
            });
        }
    }
}