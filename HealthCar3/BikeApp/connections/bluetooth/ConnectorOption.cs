using System;

namespace BikeApp.connections.bluetooth
{
    public class ConnectorOption
    {
        private readonly ServerConnection sc;

        protected ConnectorOption(ServerConnection sc)
        {
            this.sc = sc;
        }

        public float Speed { get; set; }

        public int HeartRate { get; set; }

        public float Resistance { get; set; }

        protected void SetNewSpeed(float newSpeed)
        {
            if (newSpeed == 0xFFFF)
                return;

            Speed = newSpeed;
            //valueChangeListener.OnSpeedChange(speed);
            // Updates the value
            sc.UpdateSpeed(Speed);
        }

        protected void SetNewHeartRate(int newHeartRate)
        {
            // Updates the value
            HeartRate = newHeartRate;
            sc.UpdateHeartRate(newHeartRate);
        }

        public virtual void WriteResistance(float resistance)
        {
            Resistance = resistance;
            Console.WriteLine($@"Write {resistance}%");
        }
    }
}