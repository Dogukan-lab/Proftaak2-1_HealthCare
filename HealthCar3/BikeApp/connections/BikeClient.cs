using System;
using System.Threading;
using System.Windows.Forms;
using BikeApp.connections.bluetooth;
using BikeApp.vr_environment;
using SimulatorGui;
using ClientApplication;
using System.Diagnostics;

namespace BikeApp.connections
{
    public static class Client
    {
        public static MainWindow mainWindow;
        

        public static void Initialize()
        {
            var serverCon = new ServerConnection();
            SimForm simForm = null;
            mainWindow = new MainWindow();
            ConnectorOption connector = null;


            if (!serverCon.IsLoggedIn())
            {

                mainWindow.ShowDialog();

                if (mainWindow.BluetoothEnabled())
                {
                    mainWindow.Hide();
                    connector = new Bluetooth("Avans Bike AC74", "Avans Bike AC74", serverCon);
                    
                }
                else if (mainWindow.SimulatorEnabled())
                {
                    mainWindow.Hide();
                    Application.SetHighDpiMode(HighDpiMode.SystemAware);
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                    simForm = new SimForm();
                    connector = new Simulator(serverCon, simForm);
                }

                serverCon.SetConnectorOption(connector);

                if (connector is Simulator simulator)
                {
                    //Start the update thread
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
        }
    

