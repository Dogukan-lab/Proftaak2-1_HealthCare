using System;
using System.Windows;
using System.Windows.Controls;
using Server;
using System.Diagnostics;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace ClientApplication
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    
    public partial class MainWindow : Window
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public LoginEnum LoginKind { get; set; }

        public List<ClientCredentials> _clientCredentials = File.Exists(@"..\..\..\..\Server\bin\Debug\data\saved-clientdata.json") ? StorageController.LoadClientData() : null;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            LoginKind = LoginEnum.Login;
            foreach (var item in _clientCredentials)
            {
                if((UsernameBox.Text == item._username) && (PasswordBox.Password == item._password))
                {
                    Username = item._username;
                    Password = item._password;
                    Close();
                }
            }
        }

        public bool BluetoothEnabled()
        {
            return (bool)BluetoothCheckbox.IsChecked;
        }
        public bool SimulatorEnabled()
        {
            return (bool)SimulatorCheckbox.IsChecked;
        }

        private void Register_Click(object sender, RoutedEventArgs e)
        {
            LoginKind = LoginEnum.Register;
            Username = UsernameBox.Text;
            Password = PasswordBox.Password;
            Close();
        }
    }
}
