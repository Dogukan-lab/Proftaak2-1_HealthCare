using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace DoctorGui
{
    static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            ServerConnection sc = new ServerConnection();

            while (!sc.isConnected()) { /* Wait until connected.*/ }


            while (!sc.isLoggedIn())
            {
                Console.Write("Login:\n" +
                    "username: ");
                string username = Console.ReadLine();
                Console.Write("password: ");
                string password = Console.ReadLine();

                sc.LoginToServer(username, password);
                Thread.Sleep(2000);
            }

            // Start the command thread
            Thread consoleThread = new Thread(new ParameterizedThreadStart(ReadInput));
            consoleThread.Start(sc);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new SimDocLogin());
        }

        public static void ReadInput(Object serverConnection)
        {
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
                "- stop\n" +
                "- record\n" +
                "- emergencystop\n"
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
                        sc.Broadcast(input);
                        break;
                    case "chat":
                        Console.WriteLine("Id: ");
                        idInput = Console.ReadLine();
                        Console.WriteLine("Message: ");
                        input = Console.ReadLine();
                        sc.Chat(idInput, input);
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
                    case "record":
                        Console.WriteLine("Id: ");
                        idInput = Console.ReadLine();
                        sc.GetSession(idInput);
                        break;
                    case "emergencystop":
                        sc.EmergencyStopSessions();
                        break;
                    default: // Unknown command
                        Console.WriteLine("Not a valid command.");
                        break;
                }
            }
        }
    }
}
