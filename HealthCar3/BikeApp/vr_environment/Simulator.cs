using System;
using System.Threading;
using BikeApp.connections;
using BikeApp.connections.bluetooth;
using SimulatorGui;

namespace BikeApp.vr_environment
{
    /*
     * This class is used to simulate the bluetooth bike.
     */
    public class Simulator : ConnectorOption
    {
        private readonly Random random;
        private float resistance;

        private int selectedHeartRate;
        private float selectedSpeed;
        public SimForm SimForm;
        private readonly int updateInterval = 200;

        public Simulator(ServerConnection sc, SimForm simForm) : base(sc)
        {
            // Set the base values for the speed and the heart rate
            selectedSpeed = 0;
            SpeedSway = true;
            SpeedSwayAmount = 2;
            selectedHeartRate = 80;
            HeartRateSway = true;
            HeartRateSwayAmount = 5;
            resistance = 0;
            random = new Random();
            SimForm = simForm;

            // Create a new thread that updates our values 
            UpdateThread = new Thread(UpdateValues);
        }

        public bool SpeedSway { get; }
        public int SpeedSwayAmount { get; }
        public bool HeartRateSway { get; }
        public int HeartRateSwayAmount { get; }

        public Thread UpdateThread { get; set; }

        private void UpdateValues()
        {
            if (SimForm.SpeedSwayEnabled())
            {
                // Get a random value between the selected speed +- the sway
                var minValue = Convert.ToInt32((SimForm.GetSpeed() - SimForm.GetSpeedSway()) * 100);
                var maxValue = Convert.ToInt32((SimForm.GetSpeed() + SimForm.GetSpeedSway()) * 100);
                // Clamp the value between 0 and 50 for realistic values
                var newValue = Math.Clamp(random.Next(minValue, maxValue) / 100f, 0, 50);
                SetNewSpeed(newValue);
            }
            else
            {
                SetNewSpeed(SimForm.GetSpeed());
            }

            if (SimForm.HeartRateSwayEnabled())
            {
                // Get a random value between the selected heart rate +- the sway
                var minValue = SimForm.GetHeartRate() - SimForm.GetHeartRateSway();
                var maxValue = SimForm.GetHeartRate() + SimForm.GetHeartRateSway();
                // Clamp the value between 50 and 228 for realistic values
                var newValue = Math.Clamp(random.Next(minValue, maxValue), 50, 228);
                SetNewHeartRate(newValue);
            }
            else
            {
                SetNewHeartRate(SimForm.GetHeartRate());
            }

            // Wait an amount of time to update our values again
            Thread.Sleep(updateInterval);
            UpdateValues();
        }

        public void SetSelectedSpeed(float newSelectedSpeed)
        {
            selectedSpeed = newSelectedSpeed;
        }

        public void SetSelectedHeartRate(int newSelectedHeartRate)
        {
            selectedHeartRate = newSelectedHeartRate;
        }

        /*
         * This method writes a resistance to the simulated and bluetooth bike.
         */
        public override void WriteResistance(float resistanceAmount)
        {
            resistance = Math.Clamp(resistanceAmount, 0, 100);
            base.WriteResistance(resistance);
            SimForm.SetResistance(resistance);
        }
    }
}