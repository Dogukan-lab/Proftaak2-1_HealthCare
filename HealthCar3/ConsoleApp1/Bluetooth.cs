using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Avans.TI.BLE;

namespace ConsoleApp1
{
    class Bluetooth
    {
        private int errorCode = 0;
        public BLE bleBike { get; }
        public BLE bleHeart { get; }
        public Bluetooth(string bikeID, string heartMonitorID)
        {
            // Create the BLE objects
            bleBike = new BLE();
            bleHeart = new BLE();

            // Connect to the devices
            ConnectToDevices(bikeID, heartMonitorID);
        }

        public async Task ConnectToDevices(string bikeID, string heartMonitorID)
        {
            // Connect to the bike
            errorCode = await bleBike.OpenDevice(bikeID);
            // Set the service
            errorCode = await bleBike.SetService("6e40fec1-b5a3-f393-e0a9-e50e24dcca9e");
            // Subscribe
            bleBike.SubscriptionValueChanged += BleBike_SubscriptionValueChanged;
            errorCode = await bleBike.SubscribeToCharacteristic("6e40fec2-b5a3-f393-e0a9-e50e24dcca9e");

            // Connect to the heart monitor
            errorCode = await bleHeart.OpenDevice(heartMonitorID);
            // Set the service
            errorCode = await bleHeart.SetService("HeartRate");
            // Subscribe
            bleHeart.SubscriptionValueChanged += BleBike_SubscriptionValueChanged;
            errorCode = await bleHeart.SubscribeToCharacteristic("HeartRateMeasurement");
        }

        private void BleBike_SubscriptionValueChanged(object sender, BLESubscriptionValueChangedEventArgs e)
        {
            Console.WriteLine("Received from {0}: {1}, {2}", e.ServiceName,
                BitConverter.ToString(e.Data).Replace("-", " "),
                Encoding.UTF8.GetString(e.Data));
        }
    }
}
