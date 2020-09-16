using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp1
{
    interface IValueChangeListener
    {
        public void OnSpeedChange(float speed);
        public void OnHeartRateChange(int heartRate);
    }
}
