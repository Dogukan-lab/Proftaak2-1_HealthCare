using System;
using System.Threading;
using System.Windows.Forms;
using BikeApp.connections.bluetooth;
using BikeApp.vr_environment;
using Newtonsoft.Json;
using SimulatorGui;

namespace BikeApp.connections
{
    public static class Client
    {
        public static void Initialize()
        {
            var serverCon = new ServerConnection();
            SimForm simForm = null;

            string cInput;
            while (!serverCon.IsLoggedIn())
            {
                Console.WriteLine(@"Select: " + "\n-login\n" + "-register\n");
                cInput = Console.ReadLine();
                string pInput;
                switch (cInput)
                {
                    case "login":
                        Console.WriteLine(@"Name: ");
                        cInput = Console.ReadLine();
                        Console.WriteLine(@"Password: ");
                        pInput = Console.ReadLine();
                        serverCon.LoginToServer(cInput, pInput);
                        break;
                    case "register":
                        Console.WriteLine(@"Name: ");
                        cInput = Console.ReadLine();
                        Console.WriteLine(@"Password: ");
                        pInput = Console.ReadLine();
                        serverCon.RegisterToServer(cInput, pInput);
                        break;
                }
                Thread.Sleep(2000);
            }

            cInput = "";
            // Select connector option
            ConnectorOption connector = null;
            while (cInput == string.Empty)
            {
                Console.WriteLine(@"Select bluetooth or simulator: |B|S|");
                cInput = Console.ReadLine();
                switch (cInput?.ToUpper())
                {
                    case "B":
                        connector = new Bluetooth("Avans Bike AC74", "Avans Bike AC74", serverCon);
                        break;
                    case "S":
                        // Do the gui setup
                        Application.SetHighDpiMode(HighDpiMode.SystemAware);
                        Application.EnableVisualStyles();
                        Application.SetCompatibleTextRenderingDefault(false);
                        simForm = new SimForm();
                        connector = new Simulator(serverCon, simForm);
                        break;
                    default:
                        cInput = "";
                        break;
                }
            }
            serverCon.SetConnectorOption(connector);

            if (connector is Simulator simulator)
            {
                // Start the update thread
                simulator.UpdateThread.Start();

                // Start the gui
                Application.Run(simForm);
            }
            else
            {
                Thread.Sleep(4000);
            }
        }
    }
}