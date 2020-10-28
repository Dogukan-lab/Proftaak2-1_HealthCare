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
            ConnectorOption connector = null;
            mainWindow = new MainWindow();

            if (!serverCon.IsLoggedIn())
            {
                mainWindow.ShowDialog();

                switch (mainWindow.LoginKind)
                {
                    case ClientApplication.LoginEnum.Login:
                        if (mainWindow.BluetoothEnabled())
                        {
                            serverCon.LoginToServer(mainWindow.Username, mainWindow.Password);
                            mainWindow.Hide();
                            connector = new Bluetooth("Avans Bike AC74", "Avans Bike AC74", serverCon);

                        }
                        else if (mainWindow.SimulatorEnabled())
                        {
                            serverCon.LoginToServer(mainWindow.Username, mainWindow.Password);
                            //SimulatorGui setup
                            mainWindow.Hide();
                            Application.SetHighDpiMode(HighDpiMode.SystemAware);
                            Application.EnableVisualStyles();
                            Application.SetCompatibleTextRenderingDefault(false);
                            simForm = new SimForm();
                            connector = new Simulator(serverCon, simForm);
                        }
                        break;
                    case ClientApplication.LoginEnum.Register:
                        if (mainWindow.BluetoothEnabled())
                        {
                            serverCon.RegisterToServer(mainWindow.Username, mainWindow.Password);
                            mainWindow.Hide();
                            connector = new Bluetooth("Avans Bike AC74", "Avans Bike AC74", serverCon);

                        }
                        else if (mainWindow.SimulatorEnabled())
                        {
                            serverCon.RegisterToServer(mainWindow.Username, mainWindow.Password);
                            //SimulatorGui setup
                            mainWindow.Hide();
                            Application.SetHighDpiMode(HighDpiMode.SystemAware);
                            Application.EnableVisualStyles();
                            Application.SetCompatibleTextRenderingDefault(false);
                            simForm = new SimForm();
                            connector = new Simulator(serverCon, simForm);
                        }
                        break;
                }
            }

            serverCon.SetConnectorOption(connector);

            if (connector is Simulator simulator)
            {
                //Start the update thread
                simulator.UpdateThread.Start();

                //Start the simulatorGui
                Application.Run(simForm);
            }
            else
            {
                Thread.Sleep(4000);
            }
        }
    }
}



