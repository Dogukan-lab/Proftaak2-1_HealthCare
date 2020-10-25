using LiveCharts;
using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
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
    public partial class PatientUserControl : UserControl
    {
        private Layout layoutParent = null;

        public PatientUserControl(Layout parent)
        {
            layoutParent = parent;
            InitializeComponent();
        }

        private void UpdateResistance(object sender, RoutedEventArgs e)
        {
            int bikeId = int.Parse(((Button)sender).Name[((Button)sender).Name.Length - 1].ToString());
            string resistance = ((TextBox)FindName("ResistanceLabel" + bikeId)).Text;

            var validCharacters = new Regex(@"^[0-9]+$");
            if (validCharacters.IsMatch(resistance))
                layoutParent.UpdateResistance(bikeId, resistance);
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void StartSession(object sender, RoutedEventArgs e)
        {
            int bikeId = int.Parse(((Button)sender).Name[((Button)sender).Name.Length - 1].ToString());
            layoutParent.StartSession(bikeId);
        }

        private void StopSession(object sender, RoutedEventArgs e)
        {
            int bikeId = int.Parse(((Button)sender).Name[((Button)sender).Name.Length - 1].ToString());
            layoutParent.StopSession(bikeId);
        }

        private void OnKeyDownEvent(object sender, KeyEventArgs e)
        {
            TextBox textBox = ((TextBox)sender);
            int bikeId = int.Parse(textBox.Name[textBox.Name.Length - 1].ToString());
            if (e.Key == Key.Return)
            {
                layoutParent.SendChat(bikeId, textBox.Text);
                Label newLabel = new Label();
                newLabel.Content = textBox.Text;
                ((StackPanel)FindName("ChatView" + bikeId)).Children.Add(newLabel);

                textBox.Text = "";
            }
        }
    }
}
