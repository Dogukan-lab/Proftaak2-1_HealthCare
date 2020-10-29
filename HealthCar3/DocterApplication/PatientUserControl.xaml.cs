using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using LiveCharts.Wpf.Charts.Base;

namespace DocterApplication
{
    public partial class PatientUserControl : UserControl
    {
        private readonly Layout layoutParent;

        public PatientUserControl(Layout parent)
        {
            layoutParent = parent;
            InitializeComponent();
        }

        private void UpdateResistance(object sender, RoutedEventArgs e)
        {
            var bikeId = int.Parse(((Button) sender).Name[((Button) sender).Name.Length - 1].ToString());
            var resistance = ((TextBox) FindName("ResistanceLabel" + bikeId)).Text;

            var validCharacters = new Regex(@"^[0-9]+$");
            if (validCharacters.IsMatch(resistance))
                layoutParent.UpdateResistance(bikeId, resistance);
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            var regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void StartSession(object sender, RoutedEventArgs e)
        {
            var bikeId = int.Parse(((Button) sender).Name[((Button) sender).Name.Length - 1].ToString());
            layoutParent.StartSession(bikeId);
        }

        private void StopSession(object sender, RoutedEventArgs e)
        {
            var bikeId = int.Parse(((Button) sender).Name[((Button) sender).Name.Length - 1].ToString());
            layoutParent.StopSession(bikeId);
            ResetGuiValues(bikeId);
        }

        private void OnKeyDownEvent(object sender, KeyEventArgs e)
        {
            var textBox = (TextBox) sender;
            var bikeId = int.Parse(textBox.Name[textBox.Name.Length - 1].ToString());
            if (e.Key == Key.Return)
            {
                layoutParent.SendChat(bikeId, textBox.Text);
                var newLabel = new Label();
                newLabel.Content = textBox.Text;
                ((StackPanel) FindName("ChatView" + bikeId)).Children.Add(newLabel);

                textBox.Text = "";
            }
        }

        private void ResetGuiValues(int bikeId)
        {
            Dispatcher.Invoke(delegate
            {
                ((Label) FindName("HeartrateLabel" + bikeId)).Content = "0 BPM";
                ((Label) FindName("HeartrateAverageLabel" + bikeId)).Content = "0 BPM";
                ((Label) FindName("SpeedLabel" + bikeId)).Content = "0 m/s";
                ((Label) FindName("SpeedAverageLabel" + bikeId)).Content = "0 m/s";
                ((TextBox) FindName("ResistanceLabel" + bikeId)).Text = "0";
                ((Chart) FindName("HeartRateChart" + bikeId)).Series[0].Values.Clear();
                ((Chart) FindName("SpeedChart" + bikeId)).Series[0].Values.Clear();
                layoutParent.ResetGui(bikeId);
            });
        }
    }
}