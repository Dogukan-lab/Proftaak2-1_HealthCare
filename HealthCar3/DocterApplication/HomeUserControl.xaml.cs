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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DocterApplication
{
    /// <summary>
    /// Interaction logic for HomeUserControl.xaml
    /// </summary>
    public partial class HomeUserControl : UserControl
    {
        private Layout layoutParent;
        public HomeUserControl(Layout parent)
        {
            InitializeComponent();
            layoutParent = parent;
        }

        private void Emergency_Click(object sender, RoutedEventArgs e)
        {
            layoutParent.EmergencyStop();
        }

        private void Broadcast_Click(object sender, RoutedEventArgs e)
        {
            string message = ((TextBox)FindName("BroadcastBox")).Text;
            layoutParent.BroadCast(message);
        }
    }
}
