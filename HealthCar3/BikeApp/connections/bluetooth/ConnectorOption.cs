using System;

namespace BikeApp.connections.bluetooth
{
    internal class ConnectorOption
    {
        private float speed;
        public float Speed
        {
            get { return speed;}
            set { speed = value; }
        }

        private int heartRate;
        public int HeartRate
        {
            get { return heartRate; }
            set { heartRate = value; }
        }

        private float _resistance;

        public float Resistance
        {
            get { return _resistance; }
            set { _resistance = value; }
        }


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
            // Updates the value
            heartRate = newHeartRate;
            sc.UpdateHeartRate(newHeartRate);
        }

        public virtual void WriteResistance(float resistance)
        {
            _resistance = resistance;
            Console.WriteLine($@"Write {resistance}%");
        }
    }
}
