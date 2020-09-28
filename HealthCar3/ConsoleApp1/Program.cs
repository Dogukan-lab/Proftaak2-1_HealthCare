using Newtonsoft.Json;
using SimulatorGui;
using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

using System.IO;
using System.Reflection;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {

            TempListenerClass listener = new TempListenerClass();
            SimForm simForm = null;

            // Select connector option
            ConnectorOption connector = null;
            string cInput = "";
            while (cInput == string.Empty)
            {
                Console.WriteLine("Select bluetooth or simulator: |B|S|");
                cInput = Console.ReadLine();
                if (cInput.ToUpper() == "B")
                    connector = new Bluetooth("Avans Bike AC74", "Avans Bike AC74", listener);
                else if (cInput.ToUpper() == "S")
                {
                    // Do the gui setup
                    Application.SetHighDpiMode(HighDpiMode.SystemAware);
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                    simForm = new SimForm();
                    connector = new Simulator(listener, simForm);
                }
                else
                    cInput = "";
            }


            var simulator = connector as Simulator;
            if (simulator != null)
            {
                // Start the update thread
                simulator.updateThread.Start();

                // Start the command thread
                Thread consoleThread = new Thread(new ParameterizedThreadStart(ReadInput));
                consoleThread.Start(connector);

                // Start the gui
                Application.Run(simForm);
            }
            else
            {
                Thread.Sleep(4000);
                // No need to run this on this different thread
                ReadInput(connector);
            }
        }

        public static void ReadInput(Object connectorOption)
        {
            var connector = connectorOption as ConnectorOption;

            string input = "";
            while (!input.Equals("quit"))
            {
                Console.WriteLine("Commands: \n" +
                "- quit (Quit application)\n" +
                "- res (Send resistance)");

                input = Console.ReadLine();
                switch (input)
                {
                    case "quit": // Quit the application
                        return;
                    case "res": // Send resistance to the bike
                        Console.WriteLine("Amount of resistance: ");
                        input = Console.ReadLine();
                        connector.WriteResistance(float.Parse(input));
                        Console.WriteLine("");
                        break;
                    default: // Unknown command
                        Console.WriteLine("Not a valid command.");
                        break;
                }
            }
        }

        /*VpnConnector connector = new VpnConnector(new JsonSerializerSettings());*/
    }
    }


