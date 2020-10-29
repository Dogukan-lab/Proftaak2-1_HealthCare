using System.Windows;
using System.Windows.Controls;

namespace DocterApplication
{
    /// <summary>
    ///     Interaction logic for HomeUserControl.xaml
    /// </summary>
    public partial class HomeUserControl : UserControl
    {
        private readonly Layout layoutParent;

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
            var message = ((TextBox) FindName("BroadcastBox")).Text;
            ((TextBox) FindName("BroadcastBox")).Text = "";
            layoutParent.BroadCast(message);
        }
    }
}