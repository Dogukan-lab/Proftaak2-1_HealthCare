using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace Server
{
    class StorageController
    {
        /**
         * Saves the sessionData to a file.
         */
        public static void Save(List<SessionData> data)
        {
            using var file = File.CreateText(@"..\data\saved-records.json");
            var serializer = new JsonSerializer();
            serializer.Serialize(file, data);
        }

        public static List<SessionData> Load()
        {
            if (File.Exists(@"..\data\saved-records.json"))
            {
                var serializer = new JsonSerializer();
                using var file = new StreamReader(@"..\data\saved-records.json");
                using var jsonTextReader = new JsonTextReader(file);
                var userData = serializer.Deserialize<List<SessionData>>(jsonTextReader) ?? new List<SessionData>();
                return userData;
            }
            else
            {
                Directory.CreateDirectory(@"..\data");
                File.CreateText(@"..\data\saved-records.json");
                return new List<SessionData>();
            }
        }
    }
}