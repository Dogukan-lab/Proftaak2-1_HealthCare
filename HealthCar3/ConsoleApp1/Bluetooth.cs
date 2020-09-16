using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Avans.TI.BLE;

namespace ConsoleApp1
{
    class Bluetooth : ConnectorOption
    {
        private int errorCode = 0;
        private Decoder decoder;
        public BLE bleBike { get; }
        public BLE bleHeart { get; }
        public Bluetooth(string bikeID, string heartMonitorID, IValueChangeListener listener) : base(listener)
        {
            decoder = new Decoder();

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
            //Console.WriteLine("Received from {0}: {1}, {2}", e.ServiceName,
            //    BitConverter.ToString(e.Data).Replace("-", " "),
            //    Encoding.UTF8.GetString(e.Data));

            if(e.Data[0] == 0x00)
            {
                SetNewHeartRate(decoder.DecodeHeartMonitor(e));
            }
            else if (e.Data[4] == 0x10)
            {
                SetNewSpeed(decoder.DecodeBike(e));
            }
        }

        public async override void WriteResistance(float resistance)
        {
            base.WriteResistance(resistance);
            byte byteResistance = Convert.ToByte(resistance * 2);
            byte[] data = { 0x4A, 0x09, 0x4E, 0x05, 0x30, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, byteResistance, 0x00};
            byte sumByte = data[0];
            for(int i = 1; i < data.Length-1; i++)
            {
                sumByte ^= data[i];
            }
            data[data.Length - 1] = sumByte;
            await bleBike.WriteCharacteristic("6e40fec3-b5a3-f393-e0a9-e50e24dcca9e", data);
        }
    }
}
