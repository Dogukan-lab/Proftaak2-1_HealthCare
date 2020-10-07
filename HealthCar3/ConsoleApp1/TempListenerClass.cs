using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp1
{
    class TempListenerClass : IValueChangeListener
    {
        public void OnHeartRateChange(int heartRate)
        {
            //Console.WriteLine($"{heartRate} BPM");
            // Sends sim data to the server.
        }

        public void OnSpeedChange(float speed)
        {
            //Console.WriteLine($"{speed} m/s");
            // Sends sim data to the server.
        }
    }
}
