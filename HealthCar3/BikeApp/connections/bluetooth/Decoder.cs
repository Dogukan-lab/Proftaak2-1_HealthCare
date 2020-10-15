using Avans.TI.BLE;

namespace BikeApp.connections.bluetooth
{
    internal class Decoder
    {
        private int sumByte;

        public static int DecodeHeartMonitor(BLESubscriptionValueChangedEventArgs e)
        {
            // Get the new heart rate value
            int heartRate = e.Data[1];
            return heartRate;
        }

        public float DecodeBike(BLESubscriptionValueChangedEventArgs e)
        {
            // Loop through all the received bytes except for the last one
            sumByte = e.Data[0];
            for(var i = 1; i < e.Data.Length - 1; i++)
            {
                // XOR all the bytes
                sumByte ^= e.Data[i];
            }
            // Check if the received sum is the same as our calculated one
            if (sumByte != e.Data[^1])
                return 0xFFFF;

            // Set the new speed value
            var speed = (e.Data[8] + (e.Data[9] << 8)) / 100f;
            return speed;
        }
    }
}
