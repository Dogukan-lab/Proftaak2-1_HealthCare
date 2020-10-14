using System;
using System.Collections.Generic;
using System.Text;

namespace BikeApp
{
    interface IValueChangeListener
    {
        public void OnSpeedChange(float speed);
        public void OnHeartRateChange(int heartRate);
    }
}
