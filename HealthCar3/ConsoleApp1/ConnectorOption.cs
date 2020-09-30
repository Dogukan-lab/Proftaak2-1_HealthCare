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

        public ConnectorOption(IValueChangeListener listener)
        {
            valueChangeListener = listener;
        }

        protected void SetNewSpeed(float newSpeed)
        {
            if (newSpeed == 0xFFFF)
                return;

            speed = newSpeed;
            //valueChangeListener.OnSpeedChange(speed);
        }
        protected void SetNewHeartRate(int newHeartRate)
        {
            heartRate = newHeartRate;
            //valueChangeListener.OnHeartRateChange(heartRate);
        }

        public virtual void WriteResistance(float resistance)
        {
            Console.WriteLine($"Write {resistance}%");
        }
    }
}
