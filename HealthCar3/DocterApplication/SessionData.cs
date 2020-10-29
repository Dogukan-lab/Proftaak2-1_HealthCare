using System;

namespace DocterApplication
{
    /*
     * Dataclass that holds the data of a session
     */
    public class SessionData
    {
        public SessionData(dynamic data)
        {
            clientId = data.clientId;
            name = data.name;
            sessionStart = data.sessionStart;
            sessionEnd = data.sessionEnd;
            maxHeartRate = data.maxHeartRate;
            maxSpeed = data.maxSpeed;
            averageSpeed = data.averageSpeed;
            averageHeartRate = data.averageHeartRate;
            maxResistance = data.maxResistance;
            lastResistance = data.lastResistance;
        }

        public string clientId { get; set; }
        public string name { get; set; }
        public DateTime sessionStart { get; set; }
        public DateTime sessionEnd { get; set; }
        public int maxHeartRate { get; set; }
        public int maxSpeed { get; set; }
        public int averageSpeed { get; set; }
        public int averageHeartRate { get; set; }
        public int maxResistance { get; set; }
        public int lastResistance { get; set; }

        /*
         * Puts all the data of the session into a dynamic and returns it.
         */
        public dynamic GetData()
        {
            return new
            {
                clientId,
                name,
                sessionStart,
                sessionEnd,
                maxHeartRate,
                maxSpeed,
                averageHeartRate,
                averageSpeed,
                maxResistance,
                lastResistance
            };
        }
    }
}