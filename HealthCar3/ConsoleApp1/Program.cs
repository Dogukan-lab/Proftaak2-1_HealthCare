﻿using Newtonsoft.Json;
using SimulatorGui;
using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

using System.IO;
using System.Reflection;

using DoctorGui;
namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            TempListenerClass listener = new TempListenerClass();
            ServerConnection serverCon = new ServerConnection();
            SimForm simForm = null;

            string cInput = "";
            string pInput = "";
            while (!serverCon.IsLoggedIn())
            {
                Console.WriteLine("Select: \n" +
                    "- login\n" +
                    "- register\n");
                cInput = Console.ReadLine();
                if (cInput == "login")
                {
                    Console.WriteLine("Name: ");
                    cInput = Console.ReadLine();
                    Console.WriteLine("Password: ");
                    pInput = Console.ReadLine();
                    serverCon.LoginToServer(cInput, pInput);
                }
                else if (cInput == "register")
                {
                    Console.WriteLine("Name: ");
                    cInput = Console.ReadLine();
                    Console.WriteLine("Password: ");
                    pInput = Console.ReadLine();
                    serverCon.RegisterToServer(cInput, pInput);
                }
                Thread.Sleep(2000);
            }

            cInput = "";
            // Select connector option
            ConnectorOption connector = null;
            while (cInput == string.Empty)
            {
                Console.WriteLine("Select bluetooth or simulator: |B|S|");
                cInput = Console.ReadLine();
                if (cInput.ToUpper() == "B")
                    connector = new Bluetooth("Avans Bike AC74", "Avans Bike AC74", listener, serverCon);
                else if (cInput.ToUpper() == "S")
                {
                    // Do the gui setup
                    Application.SetHighDpiMode(HighDpiMode.SystemAware);
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                    simForm = new SimForm();
                    connector = new Simulator(listener, serverCon, simForm);
                }
                else
                    cInput = "";
            }

            serverCon.SetConnectorOption(connector);

            var simulator = connector as Simulator;
            if (simulator != null)
            {
                // Start the update thread
                simulator.updateThread.Start();

                // Start the gui
                Application.Run(simForm);
            }
            else
            {
                Thread.Sleep(4000);
            }
        }
        /*VpnConnector connector = new VpnConnector(new JsonSerializerSettings());*/
    }
}


