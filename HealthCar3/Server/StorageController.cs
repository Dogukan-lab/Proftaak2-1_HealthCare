using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace Server
{
    internal static class StorageController
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

        /**
         * Saves the clientData to a file.
         */
        public static void SaveClientData(List<ClientData> data)
        {
            using var file = File.CreateText(@"..\data\saved-clientdata.json");
            var serializer = new JsonSerializer();
            serializer.Serialize(file, data);
        }


        /**
         * Loads the sessionData from a file.
         */
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
            Directory.CreateDirectory(@"..\data"); //creates the directory to prevent errors.
            File.CreateText(@"..\data\saved-records.json");
            return new List<SessionData>();
        }

        /**
         * Loads the clientData from a file.
         */
        public static List<ClientData> LoadClientData()
        {
            if (File.Exists(@"..\data\saved-clientdata.json"))
            {
                var serializer = new JsonSerializer();
                using var file = new StreamReader(@"..\data\saved-clientdata.json");
                using var jsonTextReader = new JsonTextReader(file);
                var clientData = serializer.Deserialize<List<ClientData>>(jsonTextReader) ?? new List<ClientData>();
                return clientData;
            }
            Directory.CreateDirectory(@"..\data"); //creates the directory to prevent errors.
            File.CreateText(@"..\data\saved-clientdata.json");
            return new List<ClientData>();
        }
    }
}