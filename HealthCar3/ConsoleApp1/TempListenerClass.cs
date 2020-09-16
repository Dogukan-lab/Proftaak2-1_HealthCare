using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp1
{
    class TempListenerClass : IValueChangeListener
    {
        public void OnHeartRateChange(int heartRate)
        {
            Console.WriteLine($"{heartRate} BPM");
        }

        public void OnSpeedChange(float speed)
        {
            Console.WriteLine($"{speed} m/s");
        }
    }
}
