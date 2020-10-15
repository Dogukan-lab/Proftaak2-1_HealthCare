using System;
using System.Threading.Tasks;
using Avans.TI.BLE;

namespace BikeApp.connections.bluetooth
{
    internal class Bluetooth : ConnectorOption
    {
        private int errorCode;
        private readonly Decoder decoder;
        private BLE BleBike { get; }
        private BLE BleHeart { get; }
        public Bluetooth(string bikeId, string heartMonitorId, ServerConnection sc) : base(sc)
        {
            decoder = new Decoder();
            BleBike = new BLE();
            BleHeart = new BLE();

            // Connect to the devices
            var connectionTask = ConnectToDevices(bikeId, heartMonitorId);
            connectionTask.Start();
        }
        
        private async Task ConnectToDevices(string bikeId, string heartMonitorId)
        {
            try
            {
                // Connect to the bike
                errorCode = await BleBike.OpenDevice(bikeId);
                // Set the service
                errorCode = await BleBike.SetService("6e40fec1-b5a3-f393-e0a9-e50e24dcca9e");
                // Subscribe
                BleBike.SubscriptionValueChanged += BleBike_SubscriptionValueChanged;
                errorCode = await BleBike.SubscribeToCharacteristic("6e40fec2-b5a3-f393-e0a9-e50e24dcca9e");

                // Connect to the heart monitor
                errorCode = await BleHeart.OpenDevice(heartMonitorId);
                // Set the service
                errorCode = await BleHeart.SetService("HeartRate");
                // Subscribe
                BleHeart.SubscriptionValueChanged += BleBike_SubscriptionValueChanged;
                errorCode = await BleHeart.SubscribeToCharacteristic("HeartRateMeasurement");
            }
            catch (ApplicationException e)
            {
                throw new ApplicationException(e.ToString() + errorCode);
            }
        }

        private void BleBike_SubscriptionValueChanged(object sender, BLESubscriptionValueChangedEventArgs e)
        {
            if(e.Data[0] == 0x00) // if first byte 0x00 we got the heart rate package
            {
                SetNewHeartRate(Decoder.DecodeHeartMonitor(e));
            }
            else if (e.Data[4] == 0x10) // if fifth byte 0x10 we got the bike package
            {
                SetNewSpeed(decoder.DecodeBike(e));
            }
        }

        public override async void WriteResistance(float resistance)
        {
            var byteResistance = Convert.ToByte(Math.Clamp(resistance * 2, 0, 200));
            base.WriteResistance(byteResistance / 2f);
            byte[] data = { 0x4A, 0x09, 0x4E, 0x05, 0x30, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, byteResistance, 0x00};
            // Calculate our sumbyte
            var sumByte = data[0];
            for(var i = 1; i < data.Length-1; i++)
            {
                sumByte ^= data[i];
            }
            data[^1] = sumByte;
            // Write the bytes
            await BleBike.WriteCharacteristic("6e40fec3-b5a3-f393-e0a9-e50e24dcca9e", data);
        }
    }
}
