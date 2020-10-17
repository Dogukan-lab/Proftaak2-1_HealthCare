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
    /// Interaction logic for HomePage.xaml
    /// </summary>
    public partial class Layout : Window
    {
        public Layout(String Username)
        {
            InitializeComponent();

            WelcomeDocter.Text = "Welcome Doctor " + Username;
            
            HomeUserControl.Visibility = Visibility.Visible;

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

        
        private void PatientButton_Click(object sender, RoutedEventArgs e)
        {
            
            HomeUserControl.Visibility = Visibility.Hidden;
            HistoryUserControl.Visibility = Visibility.Hidden;

            PatientUserControl.Visibility = Visibility.Visible;
           
        }

        private void HistoryButton_Click(object sender, RoutedEventArgs e)
        {
            HomeUserControl.Visibility = Visibility.Hidden;
            PatientUserControl.Visibility = Visibility.Hidden;

            HistoryUserControl.Visibility = Visibility.Visible;

        }

        private void HomeButton_Click(object sender, RoutedEventArgs e)
        {
            PatientUserControl.Visibility = Visibility.Hidden;
            HistoryUserControl.Visibility = Visibility.Hidden;

            HomeUserControl.Visibility = Visibility.Visible;
        }
    }
}
