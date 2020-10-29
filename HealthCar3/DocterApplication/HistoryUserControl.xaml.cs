using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Input;

namespace DocterApplication
{
    /// <summary>
    ///     Interaction logic for HistoryUserControl.xaml
    /// </summary>
    public partial class HistoryUserControl : UserControl
    {
        private Layout layoutParent;

        public HistoryUserControl(Layout parent)
        {
            InitializeComponent();
            layoutParent = parent;
        }

        public List<SessionData> Records { get; set; }

        private void ListBoxItem_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var item = sender as ListBoxItem;
            if (item.IsSelected)
                if (item != null)
                {
                    var id = item.Content.ToString().Split("\t\t")[1];
                    FillRecordComboBox(id);
                }
        }

        private void FillRecordComboBox(string clientId)
        {
            var personalRecords = new List<string>();
            foreach (var record in Records)
                if (record.ClientId == clientId)
                    personalRecords.Add($"{record.SessionStart:dd/MM/yy H:mm:ss}");

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
            foreach (var record in Records)
                if ($"{record.SessionStart:dd/MM/yy H:mm:ss}" == startDateTime)
                {
                    selectedRecord = record;
                    break;
                }

            Dispatcher.Invoke(delegate
            {
                HeartrateLabel.Content = $"{selectedRecord.MaxHeartRate} BPM";
                HeartrateAverageLabel.Content = $"{selectedRecord.AverageHeartRate} BPM";
                SpeedLabel.Content = $"{selectedRecord.MaxSpeed} m/s";
                SpeedAverageLabel.Content = $"{selectedRecord.AverageSpeed} m/s";
                ResistanceLabel.Content = $"{selectedRecord.MaxResistance}%";
                StartDateLabel.Content = $"{selectedRecord.SessionStart:H:mm:ss}";
                StopDateLabel.Content = $"{selectedRecord.SessionEnd:H:mm:ss}";
            });
        }
    }
}