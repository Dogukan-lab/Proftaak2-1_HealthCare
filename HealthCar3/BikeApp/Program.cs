using System;
using BikeApp.connections;

namespace BikeApp
{
    public static class Program
    {
        [STAThread]
        private static void Main()
        {
            Client.Initialize();
        }
    }
}