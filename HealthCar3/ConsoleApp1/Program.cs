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
            ServerConnection serverCon = new ServerConnection();
            SimForm simForm = null;
            //Application.Run(new SimDocLogin());

            string cInput = "";
            Console.WriteLine("Select: \n" +
                "- login\n" +
                "- register\n");
            cInput = Console.ReadLine();
            if (cInput == "login")
            {
                Console.WriteLine("Name: ");
                cInput = Console.ReadLine();
                Console.WriteLine("Id: ");
                string idInput = Console.ReadLine();
                serverCon.LoginToServer(cInput, idInput);
            }
            else if (cInput == "register")
            {
                Console.WriteLine("Name: ");
                cInput = Console.ReadLine();
                serverCon.RegisterToServer(cInput);
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

                // Start the command thread
                Thread consoleThread = new Thread(new ParameterizedThreadStart(ReadInput));
                consoleThread.Start(serverCon);

                // Start the gui
                Application.Run(simForm);
            }
            else
            {
                Thread.Sleep(4000);
                // No need to run this on this different thread
                ReadInput(serverCon);
            }
        }

        public static void ReadInput(Object serverConnection)
        {
            //var connector = connectorOption as ConnectorOption;
            var sc = serverConnection as ServerConnection;

            string input = "";
            string idInput = "";

            while (!input.Equals("quit"))
            {
                Console.WriteLine("Commands: \n" +
                "- quit (Quit application)\n" +
                "- chat\n" +
                "- broadcast\n" +
                "- resistance\n" +
                "- start\n" +
                "- stop\n"
                //"- res (Send resistance)"
                );

                input = Console.ReadLine();
                switch (input)
                {
                    case "quit": // Quit the application
                        return;
                    case "broadcast":
                        Console.WriteLine("Message: ");
                        input = Console.ReadLine();
                        sc.BroadcastTest(input);
                        break;
                    case "chat":
                        Console.WriteLine("Id: ");
                        idInput = Console.ReadLine();
                        Console.WriteLine("Message: ");
                        input = Console.ReadLine();
                        sc.ChatTest(idInput, input);
                        break;
                    case "resistance": // Send resistance to the bike
                        Console.WriteLine("Id: ");
                        idInput = Console.ReadLine();
                        Console.WriteLine("Amount of resistance: ");
                        input = Console.ReadLine();
                        sc.SetNewResistance(idInput, input);
                        Console.WriteLine("");
                        break;
                    case "start":
                        Console.WriteLine("Id: ");
                        idInput = Console.ReadLine();
                        sc.StartSession(idInput);
                        break;
                    case "stop":
                        Console.WriteLine("Id: ");
                        idInput = Console.ReadLine();
                        sc.StopSession(idInput);
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


