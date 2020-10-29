using System.Collections.Generic;
using System.IO;
using System.Windows;
using Server;

namespace ClientApplication
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly List<ClientCredentials> clientCredentials =
            File.Exists(@"..\..\..\..\Server\bin\Debug\data\saved-clientdata.json")
                ? StorageController.LoadClientData()
                : null;
        
        public string Username { get; set; }
        public string Password { get; set; }
        public LoginEnum LoginKind { get; set; }

        public MainWindow()
        {
            InitializeComponent();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            if(CheckCredentials(UsernameBox.Text, PasswordBox.Password))
            {
                Username = UsernameBox.Text;
                Password = PasswordBox.Password;
                LoginKind = LoginEnum.Login;
                Close();
            }
        }

        public bool BluetoothEnabled()
        {
            return (bool) BluetoothCheckbox.IsChecked;
        }

        public bool SimulatorEnabled()
        {
            return (bool) SimulatorCheckbox.IsChecked;
        }

        private void Register_Click(object sender, RoutedEventArgs e)
        {
            if (!CheckCredentials(UsernameBox.Text, PasswordBox.Password))
            {
                Username = UsernameBox.Text;
                Password = PasswordBox.Password;
                LoginKind = LoginEnum.Register;
                Close();
            }

        }

        private bool CheckCredentials(string username, string password)
        {
            foreach (var item in clientCredentials)
            {
                if (username == item.username && password == item.password)
                {
                    return true;
                }
            }
            return false;
        }
    }
}