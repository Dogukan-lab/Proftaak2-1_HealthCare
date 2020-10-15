namespace BikeApp.interfaces
{
    interface IValueChangeListener
    {
        public void OnSpeedChange(float speed);
        public void OnHeartRateChange(int heartRate);
    }
}
