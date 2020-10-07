namespace Server
{
    /*
     * Dataclass that holds the data of a session
     */
    class SessionData
    {
        public string clientId { get; set; }
        private int maxHeartRate { get; set; }
        private float maxSpeed { get; set; }
        private float averageSpeed { get; set; }
        private float averageHeartRate { get; set; }

        #region // Helper variables
        private int sumHeartRate = 0;
        private float sumSpeed = 0;
        private int countHeartRate = 0;
        private int countSpeed = 0;
        #endregion

        /*
         * Calculates new average and checks for new max value for speed.
         */
        public void newSpeed(float newSpeed)
        {
            countSpeed++;
            sumSpeed += newSpeed;

            if (newSpeed > maxSpeed)
                maxSpeed = newSpeed;

            averageSpeed = sumSpeed / countSpeed;
        }

        /*
         * Calculates new average and checks for new max value for heart rate.
         */
        public void newHeartRate(int newHeartRate)
        {
            countHeartRate++;
            sumHeartRate += newHeartRate;

            if (newHeartRate > maxHeartRate)
                maxHeartRate = newHeartRate;

            averageHeartRate = sumHeartRate / countHeartRate;
        }

        /*
         * Puts all the data of the session into a dynamic and returns it.
         */
        public dynamic GetData()
        {
            return new
            {
                clientId = clientId,
                maxHeartRate = maxHeartRate,
                maxSpeed = maxSpeed,
                averageHeartRate = averageHeartRate,
                averageSpeed = averageSpeed
            };
        }
    }
}
