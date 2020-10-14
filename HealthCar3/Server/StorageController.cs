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
            using var file = File.CreateText(@"..\data\jsontest.json");
            var serializer = new JsonSerializer();
            serializer.Serialize(file, data);
        }

        public static List<SessionData> Load()
        {
            var serializer = new JsonSerializer();
            using var file = new StreamReader(@"..\data\jsontest.json");
            using var jsonTextReader = new JsonTextReader(file);
            var userData = serializer.Deserialize<List<SessionData>>(jsonTextReader) ?? new List<SessionData>();
            return userData;
        }
    }
}