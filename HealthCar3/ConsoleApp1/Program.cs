using SimulatorGui;
using System;
using System.Threading;
using System.Windows.Forms;

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
            string input = "";
            while (input == string.Empty) {
                Console.WriteLine("Select bluetooth or simulator: |B|S|");
                input = Console.ReadLine();
                if (input.ToUpper() == "B")
                    connector = new Bluetooth("Avans Bike AC74", "Avans Bike AC74", listener);
                else if (input.ToUpper() == "S")
                {
                    // Do the gui setup
                    Application.SetHighDpiMode(HighDpiMode.SystemAware);
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                    simForm = new SimForm();
                    connector = new Simulator(listener, simForm);
                }
                else
                    input = "";
            }


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
                connector.WriteResistance(50f);
                Console.Read();
            }
            
            /*
            VpnConnector connector = new VpnConnector();
            connector.Send(new Id("session/list"));
            */
        }       
    }
}
