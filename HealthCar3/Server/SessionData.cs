using System;

namespace Server
{
    /*
     * Dataclass that holds the data of a session
     */
    public class SessionData
    {
        public string ClientId { get; set; }
        public string Name { get; set; }
        public DateTime SessionStart { get; set; }
        public DateTime SessionEnd { get; set; }
        public int MaxHeartRate { get; set; }
        public float MaxSpeed { get; set; }
        public float AverageSpeed { get; set; }
        public float AverageHeartRate { get; set; }
        public float MaxResistance { get; set; }
        public float LastResistance { get; set; }

        #region Helper variables

        private int sumHeartRate;
        private float sumSpeed;
        private int countHeartRate;
        private int countSpeed;

        #endregion
        
        /*
         * Calculates new average and checks for new max value for speed.
         */
        public void NewSpeed(float newSpeed)
        {
            countSpeed++;
            sumSpeed += newSpeed;

            if (newSpeed > MaxSpeed)
                MaxSpeed = newSpeed;

            AverageSpeed = sumSpeed / countSpeed;
        }

        /*
         * Calculates new average and checks for new max value for heart rate.
         */
        public void NewHeartRate(int newHeartRate)
        {
            countHeartRate++;
            sumHeartRate += newHeartRate;

            if (newHeartRate > MaxHeartRate)
                MaxHeartRate = newHeartRate;

            AverageHeartRate = sumHeartRate / countHeartRate;
        }

        /*
         * Updates the last resistance and checks if its the max resistance this session.
         */
        public void NewResistance(float newResistance)
        {
            LastResistance = newResistance;
            if (LastResistance > MaxResistance)
                MaxResistance = LastResistance;
        }

        /*
         * Puts all the data of the session into a dynamic and returns it.
         */
        public dynamic GetData()
        {
            return new
            {
                clientId = ClientId,
                name = Name,
                sessionStart = SessionStart,
                sessionEnd = SessionEnd,
                maxHeartRate = MaxHeartRate,
                maxSpeed = MaxSpeed,
                averageHeartRate = AverageHeartRate,
                averageSpeed = AverageSpeed,
                maxResistance = MaxResistance,
                lastResistance = LastResistance
            };
        }

    }
}