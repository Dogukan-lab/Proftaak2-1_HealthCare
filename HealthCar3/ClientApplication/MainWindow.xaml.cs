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
        private ServerConnection sc;


        public MainWindow()
        {
            InitializeComponent();
            sc = new ServerConnection();
            
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameBox.Text;
            string password = PasswordBox.Password;
            sc.LoginToServer(username, password);
            
        }

        public bool BluetoothEnabled()
        {
            return (bool)BluetoothCheckbox.IsChecked;
        }
        public bool SimulatorEnabled()
        {
            return (bool)SimulatorCheckbox.IsChecked;
        }

        

        
    }
}
