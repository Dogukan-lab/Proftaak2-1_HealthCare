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
            ClientId = data.ClientId;
            Name = data.Name;
            SessionStart = data.SessionStart;
            SessionEnd = data.SessionEnd;
            MaxHeartRate = data.MaxHeartRate;
            MaxSpeed = data.MaxSpeed;
            AverageSpeed = data.AverageSpeed;
            AverageHeartRate = data.AverageHeartRate;
            MaxResistance = data.MaxResistance;
            LastResistance = data.LastResistance;
        }

        public string ClientId { get; set; }
        public string Name { get; set; }
        public DateTime SessionStart { get; set; }
        public DateTime SessionEnd { get; set; }
        public int MaxHeartRate { get; set; }
        public int MaxSpeed { get; set; }
        public int AverageSpeed { get; set; }
        public int AverageHeartRate { get; set; }
        public int MaxResistance { get; set; }
        public int LastResistance { get; set; }

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