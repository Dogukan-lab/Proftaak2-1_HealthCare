using BikeApp.connections;
using Newtonsoft.Json;
using System;
using System.Threading;
using ClientApplication;


namespace BikeApp
{
    internal static class Program
    {
        
        [STAThread]
        private static void Main()
        {
            Client.Initialize();            
        }
    }
}


