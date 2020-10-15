using System;

namespace Server
{
    /*
     * Dataclass that holds the data of a session
     */
    class SessionData
    {
        public string clientId { get; set; }
        public string name { get; set; }
        public DateTime sessionStart { get; set; }
        public DateTime sessionEnd { get; set; }
        public int maxHeartRate { get; set; }
        public float maxSpeed { get; set; }
        public float averageSpeed { get; set; }
        public float averageHeartRate { get; set; }
        public float maxResistance { get; set; }
        public float lastResistance { get; set; }

        #region Helper variables
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
        public void NewHeartRate(int newHeartRate)
        {
            countHeartRate++;
            sumHeartRate += newHeartRate;

            if (newHeartRate > maxHeartRate)
                maxHeartRate = newHeartRate;

            averageHeartRate = sumHeartRate / countHeartRate;
        }

        /*
         * Updates the last resistance and checks if its the max resistance this session.
         */
        public void NewResistance(float newResistance)
        {
            lastResistance = newResistance;
            if (lastResistance > maxResistance)
                maxResistance = lastResistance;
        }

        /*
         * Puts all the data of the session into a dynamic and returns it.
         */
        public dynamic GetData()
        {
            return new
            {
                clientId = clientId,
                name = name,
                sessionStart = sessionStart,
                sessionEnd = sessionEnd,
                maxHeartRate = maxHeartRate,
                maxSpeed = maxSpeed,
                averageHeartRate = averageHeartRate,
                averageSpeed = averageSpeed,
                maxResistance = maxResistance,
                lastResistance = lastResistance
            };
        }
    }
}
