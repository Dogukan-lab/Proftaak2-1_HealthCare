using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace ConsoleApp1
{
    class Simulator : ConnectorOption
    {
        public bool speedSway { get; set; }
        public int speedSwayAmount { get; set; }
        public bool heartRateSway { get; set; }
        public int heartRateSwayAmount { get; set; }

        private int selectedHeartRate;
        private float selectedSpeed;
        private int updateInterval = 200;
        private Random random;
        public Thread updateThread { get; }
        public Simulator(IValueChangeListener listener) : base(listener)
        {
            // Set the base values for the speed and the heart rate
            selectedSpeed = 0;
            speedSway = true;
            speedSwayAmount = 2;

            selectedHeartRate = 80;
            heartRateSway = true;
            heartRateSwayAmount = 5;

            random = new Random();

            // Create a new thread that updates our values 
            updateThread = new Thread(new ThreadStart(UpdateValues));
        }

        private void UpdateValues()
        {
            if (speedSway)
            {
                // Get a random value between the selected speed +- the sway
                int minValue = Convert.ToInt32(selectedSpeed - speedSwayAmount) * 100;
                int maxValue = Convert.ToInt32(selectedSpeed + speedSwayAmount) * 100;
                // Clamp the value between 0 and 50 for realistic values
                float newValue = Math.Clamp(random.Next(minValue, maxValue) / 100f, 0, 50);
                SetNewSpeed(newValue);
            }
            else
            {
                SetNewSpeed(selectedSpeed);
            }

            if (heartRateSway)
            {
                // Get a random value between the selected heart rate +- the sway
                int minValue = Convert.ToInt32(selectedHeartRate - heartRateSwayAmount) * 100;
                int maxValue = Convert.ToInt32(selectedHeartRate + heartRateSwayAmount) * 100;
                // Clamp the value between 50 and 228 for realistic values
                int newValue = Math.Clamp(random.Next(minValue, maxValue) / 100, 50, 228);
                SetNewHeartRate(newValue);
            }
            else
            {
                SetNewHeartRate(selectedHeartRate);
            }

            // Wait an amount of time to update our values again
            Thread.Sleep(updateInterval);
            UpdateValues();
        }
        public void SetSelectedSpeed(float newSelectedSpeed) { selectedSpeed = newSelectedSpeed; }
        public void SetSelectedHeartRate(int newSelectedHeartRate) { selectedHeartRate = newSelectedHeartRate; }

    }
}
