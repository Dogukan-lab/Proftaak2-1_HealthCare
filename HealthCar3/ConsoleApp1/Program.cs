using System;
using System.Threading;
using System.Windows.Forms;
using SimulatorGui;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            TempListenerClass listener = new TempListenerClass();

            // Select connector option
            ConnectorOption connector = null;
            string input = "";
            while (input == string.Empty) {
                Console.WriteLine("Select bluetooth or simulator: |B|S|");
                input = Console.ReadLine();
                if (input.ToUpper() == "B")
                    connector = new Bluetooth("Avans Bike AC74", "Avans Bike AC74", listener);
                else if (input.ToUpper() == "S")
                    connector = new Simulator(listener);
                else
                    input = "";
            }


            var simulator = connector as Simulator;
            if (simulator != null)
            {
                //Console.WriteLine("Give a base speed:");
                //simulator.SetSelectedSpeed(float.Parse(Console.ReadLine()));
                //
                //Console.WriteLine("Give a base heart rate:");
                //simulator.SetSelectedHeartRate(int.Parse(Console.ReadLine()));

                // Start the gui
                Application.SetHighDpiMode(HighDpiMode.SystemAware);
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Form1 simForm = new Form1();
                Application.Run(simForm);

                

                simulator.updateThread.Start();
            }
            else
            {
                Thread.Sleep(4000);
                connector.WriteResistance(50f);
                Console.Read();
            }

        }       
    }
}
