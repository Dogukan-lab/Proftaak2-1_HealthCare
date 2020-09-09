using System;
using Avans.TI.BLE;

namespace ConsoleApp1
{
    class Decoder
    {
        private int sumByte;
        public int heartRate { get; set; }
        public float speed { get; set; }

        public void DecodeHeartMonitor(BLESubscriptionValueChangedEventArgs e)
        {
            // Set the new heart rate value
            heartRate = e.Data[1];
            Console.WriteLine($"{heartRate} BPM");
        }

        public void DecodeBike(BLESubscriptionValueChangedEventArgs e)
        {
            // Loop through all the received bytes except for the last one
            sumByte = e.Data[0];
            for(int i = 1; i < e.Data.Length - 1; i++)
            {
                // XOR all the bytes
                sumByte ^= e.Data[i];
            }
            // Check if the received sum is the same as our calculated one
            if (sumByte != e.Data[e.Data.Length - 1])
                return;

            // Set the new speed value
            speed = (e.Data[8] + (e.Data[9] << 8)) / 100f;
            Console.WriteLine($"{speed} m/s");
        }
    }
}
