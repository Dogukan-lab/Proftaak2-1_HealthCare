using System;
using System.Threading;
using System.Windows.Forms;
// ReSharper disable LocalizableElement

namespace DoctorApp
{
    internal static class Program
    {
        /*
         * The main entry point for the application.
         */
        [STAThread]
        private static void Main()
        {
            var sc = new ServerConnection();
            while (!sc.IsConnected()) { /* Wait until connected.*/ }
            while (!sc.IsLoggedIn())
            {
                Console.Write("Login" + "\nusername: ");
                var username = Console.ReadLine();
                Console.Write("password: ");
                var password = Console.ReadLine();

                sc.LoginToServer(username, password);
                Thread.Sleep(2000);
            }

            // Start the command thread
            var consoleThread = new Thread(ReadInput);
            consoleThread.Start(sc);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new SimDocLogin());
        }

        private static void ReadInput(Object serverConnection)
        {
            var sc = serverConnection as ServerConnection;

            var input = "";
            
            while (input != null && !input.Equals("quit"))
            {
                Console.WriteLine("Commands: -quit (Quit application) -chat -broadcast -resistance -start -stop -record -emergencyStop");
                input = Console.ReadLine();
                string idInput;
                switch (input)
                {
                    case "quit": // Quit the application
                        return;
                    case "broadcast":
                        Console.WriteLine("Message: ");
                        input = Console.ReadLine();
                        sc?.Broadcast(input);
                        break;
                    case "chat":
                        Console.WriteLine("Id: ");
                        idInput = Console.ReadLine();
                        Console.WriteLine("Message: ");
                        input = Console.ReadLine();
                        sc?.Chat(idInput, input);
                        break;
                    case "resistance": // Send resistance to the bike
                        Console.WriteLine("Id: ");
                        idInput = Console.ReadLine();
                        Console.WriteLine("Amount of resistance: ");
                        input = Console.ReadLine();
                        sc?.SetNewResistance(idInput, input);
                        Console.WriteLine("");
                        break;
                    case "start":
                        Console.WriteLine("Id: ");
                        idInput = Console.ReadLine();
                        sc?.StartSession(idInput);
                        break;
                    case "stop":
                        Console.WriteLine("Id: ");
                        idInput = Console.ReadLine();
                        sc?.StopSession(idInput);
                        break;
                    case "record":
                        Console.WriteLine("Id: ");
                        idInput = Console.ReadLine();
                        sc?.GetSession(idInput);
                        break;
                    case "emergencyStop":
                        sc?.EmergencyStopSessions();
                        break;
                    default: // Unknown command
                        Console.WriteLine("Not a valid command.");
                        break;
                }
            }
        }
    }
}
