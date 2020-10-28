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
    /// Interaction logic for HistoryUserControl.xaml
    /// </summary>
    public partial class HistoryUserControl : UserControl
    {
        private Layout layoutParent;
        public List<SessionData> Records { get; set; }
        public HistoryUserControl(Layout parent)
        {
            InitializeComponent();
            layoutParent = parent;
        }

        private void ListBoxItem_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var item = sender as ListBoxItem;
            if (item.IsSelected)
            {
                if (item != null)
                {
                    string id = item.Content.ToString().Split("\t\t")[1];
                    FillRecordComboBox(id);
                }
            }
        }

        private void FillRecordComboBox(string clientId)
        {
            List<string> personalRecords = new List<string>();
            foreach (var record in Records)
                if (record.clientId == clientId)
                    personalRecords.Add($"{record.sessionStart:dd/MM/yy H:mm:ss}");

            ClientRecordsComboBox.ItemsSource = personalRecords;
        }

        private void ClientRecordsComboBox_OnRecordSelect(object sender, SelectionChangedEventArgs e)
        {
            var comboBox = sender as ComboBox;
            var item = comboBox.SelectedValue;
            if (item != null)
                FillRecordValues(item.ToString());
        }

        private void FillRecordValues(string startDateTime)
        {
            SessionData selectedRecord = null;
            foreach(var record in Records)
                if($"{record.sessionStart:dd/MM/yy H:mm:ss}" == startDateTime)
                {
                    selectedRecord = record;
                    break;
                }

            Dispatcher.Invoke(delegate {
                HeartrateLabel.Content = $"{selectedRecord.maxHeartRate} BPM";
                HeartrateAverageLabel.Content = $"{selectedRecord.averageHeartRate} BPM";
                SpeedLabel.Content = $"{selectedRecord.maxSpeed} m/s";
                SpeedAverageLabel.Content = $"{selectedRecord.averageSpeed} m/s";
                ResistanceLabel.Content = $"{selectedRecord.maxResistance}%";
                StartDateLabel.Content = $"{selectedRecord.sessionStart:H:mm:ss}";
                StopDateLabel.Content = $"{selectedRecord.sessionEnd:H:mm:ss}";
            });
        }
    }
}
