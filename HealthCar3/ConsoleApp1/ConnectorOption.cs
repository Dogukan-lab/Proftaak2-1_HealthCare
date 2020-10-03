using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp1
{
    class ConnectorOption
    {
        private int heartRate;
        private float speed;
        private IValueChangeListener valueChangeListener;
        private ServerConnection sc;

        public ConnectorOption(IValueChangeListener listener, ServerConnection sc)
        {
            valueChangeListener = listener;
            this.sc = sc;
        }

        protected void SetNewSpeed(float newSpeed)
        {
            if (newSpeed == 0xFFFF)
                return;

            speed = newSpeed;
            //valueChangeListener.OnSpeedChange(speed);
            // Updates the value
            //sc.UpdateSpeed(newSpeed);
        }
        protected void SetNewHeartRate(int newHeartRate)
        {
            heartRate = newHeartRate;
            //valueChangeListener.OnHeartRateChange(heartRate);
            // Updates the value
            //sc.UpdateHeartRate(newHeartRate);
        }

        public virtual void WriteResistance(float resistance)
        {
            Console.WriteLine($"Write {resistance}%");
        }
    }
}
