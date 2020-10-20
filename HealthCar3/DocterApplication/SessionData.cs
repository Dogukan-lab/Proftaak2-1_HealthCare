using System;

namespace DocterApplication
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

        public SessionData(dynamic data)
        {
            this.clientId = data.clientId;
            this.name = data.name;
            this.sessionStart = data.sessionStart;
            this.sessionEnd = data.sessionEnd;
            this.maxHeartRate = data.maxHeartRate;
            this.maxSpeed = data.maxSpeed;
            this.averageSpeed = data.averageSpeed;
            this.averageHeartRate = data.averageHeartRate;
            this.maxResistance = data.maxResistance;
            this.lastResistance = data.lastResistance;
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
