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

namespace DocterApplication
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        private Boolean MouseEnterUsername = false;
        private Boolean MouseEnterPassword = false;
        private String Username { get; set; }

        HomePage homePage { get; set; }
        public LoginWindow()
        {
            InitializeComponent();
        }


        // Drag Window
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if(e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }

        }


        //Clear the username box
        private void TextBox_MouseEnter(object sender, MouseEventArgs e)
        {
            if (this.MouseEnterUsername == false)
            {
                UsernameBox.Clear();
                this.MouseEnterUsername = true;
            }
        }

        //Clear Password Box

        private void PasswordBox_MouseEnter(object sender, MouseEventArgs e)
        {
            if (this.MouseEnterPassword == false)
            {
                PasswordBox.Clear();
                this.MouseEnterPassword = true;
            }
        }

        //Close the Application

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }


        //Check login than open HomePage
        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            this.Username = UsernameBox.Text;

            this.homePage = new HomePage(this.Username);
            this.Hide();
            homePage.Show();
        }
    }
}
