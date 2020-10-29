using System.Threading;
using System.Windows;
using System.Windows.Input;

namespace DocterApplication
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        private bool mouseEnterUsername = false;
        private bool mouseEnterPassword = false;

        private readonly ServerConnection sc;

        public LoginWindow()
        {
            InitializeComponent();

            sc = new ServerConnection();
        }


        // Drag Window
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed) DragMove();
        }


        //Clear the username box
        private void TextBox_MouseEnter(object sender, MouseEventArgs e)
        {
            if (this.mouseEnterUsername == false)
            {
                UsernameBox.Clear();
                this.mouseEnterUsername = true;
            }
        }

        //Clear Password Box
        private void PasswordBox_MouseEnter(object sender, MouseEventArgs e)
        {
            if (this.mouseEnterPassword == false)
            {
                PasswordBox.Clear();
                this.mouseEnterPassword = true;
            }
        }

        //Close the Application
        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }


        //Check login than open HomePage
        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            var username = UsernameBox.Text;
            var password = PasswordBox.Password;
            sc.LoginToServer(username, password);

            while (!sc.HasReceivedLoginFeedback())
                Thread.Sleep(5);

            if (sc.IsLoggedIn())
            {
                Hide();
                var layout = new Layout(username, sc);
                layout.Show();
            }
            else
            {
                UsernameBox.Clear();
                PasswordBox.Clear();
            }
        }
    }
}