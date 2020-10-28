using DocterApplication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ClientApplication
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    
    public partial class MainWindow : Window
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public MainWindow()
        {
            InitializeComponent();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            Username = UsernameBox.Text;
            Password = PasswordBox.Password;
            this.Close();
        }

        public bool BluetoothEnabled()
        {
            return (bool)BluetoothCheckbox.IsChecked;
        }
        public bool SimulatorEnabled()
        {
            return (bool)SimulatorCheckbox.IsChecked;
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
            
        }
    }
}
