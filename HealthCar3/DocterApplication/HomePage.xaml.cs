﻿using System;
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
    public partial class HomePage : Window
    {
        String Username { get; set; }



        public HomePage(String Username)
        {
            InitializeComponent();
            this.Username = Username;
            WelcomeDocter.Text = "Welcome Doctor " + this.Username;

        }

        public HomePage()
        {
            InitializeComponent();
            WelcomeDocter.Text = this.Username;
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
            this.Hide();
            PatientPage patientPage = new PatientPage(Username);
            patientPage.Show();
        }

        private void HistoryButton_Click(object sender, RoutedEventArgs e)
        {

            this.Hide();
            HistoryPage historyPage = new HistoryPage(Username);
            historyPage.Show();
        }
    }
}
