using SimulatorGui;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Newtonsoft.Json;

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
        private float resistance;
        private SimForm simForm;

        //send data to server
        private readonly String IPAddress = "127.0.0.1";
        private readonly int portNum = 1330;
        private ServerConnection serverCon;
        private JsonSerializerSettings settings;

        public Thread updateThread { get; }
        public Simulator(IValueChangeListener listener, SimForm simForm) : base(listener)
        {
            // Set the base values for the speed and the heart rate
            selectedSpeed = 0;
            speedSway = true;
            speedSwayAmount = 2;

            selectedHeartRate = 80;
            heartRateSway = true;
            heartRateSwayAmount = 5;

            resistance = 0;
            random = new Random();

            this.simForm = simForm;

            //has to make server connection
            serverCon = new ServerConnection(IPAddress, portNum);
            settings = new JsonSerializerSettings();
            settings.NullValueHandling = NullValueHandling.Ignore;


            // Create a new thread that updates our values 
            updateThread = new Thread(new ThreadStart(UpdateValues));
        }

        private void UpdateValues()
        {
            if (simForm.SpeedSwayEnabled())
            {
                // Get a random value between the selected speed +- the sway
                int minValue = Convert.ToInt32((simForm.GetSpeed() - simForm.GetSpeedSway()) * 100);
                int maxValue = Convert.ToInt32((simForm.GetSpeed() + simForm.GetSpeedSway()) * 100);
                // Clamp the value between 0 and 50 for realistic values
                float newValue = Math.Clamp(random.Next(minValue, maxValue) / 100f, 0, 50);
                SetNewSpeed(newValue);
            }
            else
            {
                SetNewSpeed(simForm.GetSpeed());
            }

            if (simForm.HeartRateSwayEnabled())
            {
                // Get a random value between the selected heart rate +- the sway
                int minValue = simForm.GetHeartRate() - simForm.GetHeartRateSway();
                int maxValue = simForm.GetHeartRate() + simForm.GetHeartRateSway();
                // Clamp the value between 50 and 228 for realistic values
                int newValue = Math.Clamp(random.Next(minValue, maxValue), 50, 228);
                SetNewHeartRate(newValue);
            }
            else
            {
                SetNewHeartRate(simForm.GetHeartRate());
            }

            serverCon.Message(JsonConvert.SerializeObject(new ServerData(selectedHeartRate, selectedSpeed)));

            /*serverCon.Message(JsonConvert.SerializeObject("test message"));*/

            // Wait an amount of time to update our values again
            Thread.Sleep(updateInterval);
            UpdateValues();
        }
        public void SetSelectedSpeed(float newSelectedSpeed) { selectedSpeed = newSelectedSpeed; }
        public void SetSelectedHeartRate(int newSelectedHeartRate) { selectedHeartRate = newSelectedHeartRate; }

        public override void WriteResistance(float resistance)
        {
            this.resistance = Math.Clamp(resistance, 0, 100);
            base.WriteResistance(this.resistance);
            simForm.SetResistance(this.resistance);
        }
    }
}
