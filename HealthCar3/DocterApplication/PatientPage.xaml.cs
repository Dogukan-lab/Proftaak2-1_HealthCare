using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DocterApplication
{
    /// <summary>
    /// Interaction logic for PatientPage.xaml
    /// </summary>
    public partial class PatientPage : Window
    {
        private String Username;
        public PatientPage(String Username)
        {
            this.Username = Username;
            InitializeComponent();
        }

        //Dragging Window
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        //Close Application
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void HomeButton_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            HomePage homepage = new HomePage();
            homepage.Show();
        }

        private void HistoryButton_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            HistoryPage historypage = new HistoryPage(Username);
            historypage.Show();
        }
    }
}
