using System;

namespace BikeApp.connections.bluetooth
{
    internal class ConnectorOption
    {
        private float speed;
        private readonly ServerConnection sc;

        protected ConnectorOption(ServerConnection sc)
        {
            this.sc = sc;
        }

        protected void SetNewSpeed(float newSpeed)
        {
            if (newSpeed == 0xFFFF)
                return;

            speed = newSpeed;
            //valueChangeListener.OnSpeedChange(speed);
            // Updates the value
            sc.UpdateSpeed(speed);
        }
        protected void SetNewHeartRate(int newHeartRate)
        {
            //valueChangeListener.OnHeartRateChange(heartRate);
            // Updates the value
            sc.UpdateHeartRate(newHeartRate);
        }

        public virtual void WriteResistance(float resistance)
        {
            Console.WriteLine($@"Write {resistance}%");
        }
    }
}
