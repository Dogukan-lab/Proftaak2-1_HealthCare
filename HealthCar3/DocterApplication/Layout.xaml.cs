using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using LiveCharts.Wpf.Charts.Base;

namespace DocterApplication
{
    /// <summary>
    ///     Interaction logic for HomePage.xaml
    /// </summary>
    public partial class Layout : Window
    {
        public delegate void GuiCallBack(int bikeId, string newValue);

        private readonly List<Bike> bikes;
        private readonly HistoryUserControl historyUserControl;
        private readonly HomeUserControl homeUserControl;
        private readonly PatientUserControl patientUserControl;
        private readonly ServerConnection sc;

        private string username;

        public Layout(string username, ServerConnection sc)
        {
            InitializeComponent();

            this.username = username;
            this.sc = sc;
            sc.SetLayoutParent(this);
            WelcomeDocter.Text = "Welcome Doctor " + username;
            bikes = new List<Bike>();

            // Initialise the different pages.
            homeUserControl = new HomeUserControl(this);
            GridUserControls.Children.Add(homeUserControl);
            patientUserControl = new PatientUserControl(this);
            GridUserControls.Children.Add(patientUserControl);
            historyUserControl = new HistoryUserControl(this);
            GridUserControls.Children.Add(historyUserControl);

            homeUserControl.Visibility = Visibility.Visible;
            patientUserControl.Visibility = Visibility.Hidden;
            historyUserControl.Visibility = Visibility.Hidden;
        }

        //Dragging Window
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed) DragMove();
        }

        //Close Application
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void PatientButton_Click(object sender, RoutedEventArgs e)
        {
            homeUserControl.Visibility = Visibility.Hidden;
            historyUserControl.Visibility = Visibility.Hidden;

            patientUserControl.Visibility = Visibility.Visible;
        }

        private void HistoryButton_Click(object sender, RoutedEventArgs e)
        {
            homeUserControl.Visibility = Visibility.Hidden;
            patientUserControl.Visibility = Visibility.Hidden;

            historyUserControl.Visibility = Visibility.Visible;

            sc.GetSessions();
        }

        private void HomeButton_Click(object sender, RoutedEventArgs e)
        {
            patientUserControl.Visibility = Visibility.Hidden;
            historyUserControl.Visibility = Visibility.Hidden;

            homeUserControl.Visibility = Visibility.Visible;
        }

        public void NewClient(string id, string name)
        {
            bikes.Add(new Bike(bikes.Count + 1, id, name));

            // Update home screen labels
            SetNewGuiLabelValue(bikes.Count, name, SetHomeClientNameCB); // Update client name
            SetNewGuiLabelValue(bikes.Count, "0", SetHomeHeartRateCB); // Set heart rate to 0
            SetNewGuiLabelValue(bikes.Count, "0", SetHomeSpeedCB); // Set speed to 0
            SetNewGuiLabelValue(bikes.Count, "0", SetHomeResistanceCB); // Set resistance to 0

            // Update patient screen labels
            SetNewGuiLabelValue(bikes.Count, name, SetPatientClientNameCB); // Update client name
            SetNewGuiLabelValue(bikes.Count, "0", SetPatientHeartRateCB); // Set heart rate to 0
            SetNewGuiLabelValue(bikes.Count, "0", SetPatientAvgHeartRateCB); // Set avg heart rate to 0
            SetNewGuiLabelValue(bikes.Count, "0", SetPatientSpeedCB); // Set speed to 0
            SetNewGuiLabelValue(bikes.Count, "0", SetPatientAvgSpeedCB); // Set avg speed to 0

            // Link the charts with the data
            SetNewGuiChartLink(bikes.Count, LinkHeartRateChartCB);
            SetNewGuiChartLink(bikes.Count, LinkSpeedChartCB);
        }

        internal void ResetGui(int bikeId)
        {
            ((Label) homeUserControl.FindName("HeartrateBox" + bikeId)).Content = "0 BPM";
            ((Label) homeUserControl.FindName("SpeedBox" + bikeId)).Content = "0 m/s";
            ((Label) homeUserControl.FindName("ResistanceBox" + bikeId)).Content = "0%";
        }

        internal void RemoveClient(string clientId)
        {
            foreach (var bike in bikes)
                if (bike.ID == clientId)
                {
                    // Reset home screen labels
                    SetNewGuiLabelValue(bike.BikeId, "-", SetHomeClientNameCB); // Remove client name
                    SetNewGuiLabelValue(bike.BikeId, "0", SetHomeHeartRateCB); // Set heart rate to 0
                    SetNewGuiLabelValue(bike.BikeId, "0", SetHomeSpeedCB); // Set speed to 0
                    SetNewGuiLabelValue(bike.BikeId, "0", SetHomeResistanceCB); // Set resistance to 0

                    // Reset patient screen labels
                    SetNewGuiLabelValue(bike.BikeId, "-", SetPatientClientNameCB); // Remove client name
                    SetNewGuiLabelValue(bike.BikeId, "0", SetPatientHeartRateCB); // Set heart rate to 0
                    SetNewGuiLabelValue(bike.BikeId, "0", SetPatientAvgHeartRateCB); // Set avg heart rate to 0
                    SetNewGuiLabelValue(bike.BikeId, "0", SetPatientSpeedCB); // Set speed to 0
                    SetNewGuiLabelValue(bike.BikeId, "0", SetPatientAvgSpeedCB); // Set avg speed to 0

                    //Resets the chatbox inside of the patient control view
                    SetNewGuiLabelValue(bike.BikeId, "", ClearChatBox);

                    bikes.Remove(bike);
                    return;
                }
        }

        public void NewHeartRate(string id, int newHeartRate)
        {
            foreach (var bike in bikes)
                if (bike.ID == id)
                {
                    bike.NewHeartRate(newHeartRate);

                    // Check if y axis need to adjust to the new value.
                    Dispatcher.Invoke(delegate
                    {
                        if (newHeartRate > 125)
                        {
                            ((Chart) patientUserControl.FindName("HeartRateChart" + bike.BikeId)).AxisY[0].MinValue =
                                100;
                            ((Chart) patientUserControl.FindName("HeartRateChart" + bike.BikeId)).AxisY[0].MaxValue =
                                225;
                        }
                        else
                        {
                            ((Chart) patientUserControl.FindName("HeartRateChart" + bike.BikeId)).AxisY[0].MinValue =
                                50;
                            ((Chart) patientUserControl.FindName("HeartRateChart" + bike.BikeId)).AxisY[0].MaxValue =
                                175;
                        }
                    });

                    // Update the GUI with the new values.
                    SetNewGuiLabelValue(bike.BikeId, newHeartRate.ToString(), SetHomeHeartRateCB);
                    SetNewGuiLabelValue(bike.BikeId, bike.CurrentHeartRate.ToString(), SetPatientHeartRateCB);
                    SetNewGuiLabelValue(bike.BikeId, bike.AverageHeartRate.ToString(), SetPatientAvgHeartRateCB);
                }
        }

        public void NewSpeed(string id, int newSpeed)
        {
            foreach (var bike in bikes)
                if (bike.ID == id)
                {
                    bike.NewSpeed(newSpeed);

                    // Check if y axis need to adjust to the new value.
                    Dispatcher.Invoke(delegate
                    {
                        if (newSpeed > 25)
                        {
                            ((Chart) patientUserControl.FindName("SpeedChart" + bike.BikeId)).AxisY[0].MinValue = 25;
                            ((Chart) patientUserControl.FindName("SpeedChart" + bike.BikeId)).AxisY[0].MaxValue = 50;
                        }
                        else
                        {
                            ((Chart) patientUserControl.FindName("SpeedChart" + bike.BikeId)).AxisY[0].MinValue = 0;
                            ((Chart) patientUserControl.FindName("SpeedChart" + bike.BikeId)).AxisY[0].MaxValue = 25;
                        }
                    });

                    // Update the GUI with the new values.
                    SetNewGuiLabelValue(bike.BikeId, newSpeed.ToString(), SetHomeSpeedCB);
                    SetNewGuiLabelValue(bike.BikeId, bike.CurrentSpeed.ToString(), SetPatientSpeedCB);
                    SetNewGuiLabelValue(bike.BikeId, bike.AverageSpeed.ToString(), SetPatientAvgSpeedCB);
                }
        }

        internal void RefreshHistoryPage(List<SessionData> records)
        {
            SetNewGuiListValue(records, SetHistoryClientBoxCB);
        }

        public void SetNewGuiLabelValue(int bikeId, string newValue, GuiCallBack callback)
        {
            Application.Current.Dispatcher.Invoke(callback, bikeId, newValue);
        }

        internal void StartSession(int bikeId)
        {
            foreach (var bike in bikes)
                if (bike.BikeId == bikeId)
                    sc.StartSession(bike.ID);
        }

        internal void StopSession(int bikeId)
        {
            foreach (var bike in bikes)
                if (bike.BikeId == bikeId)
                    sc.StopSession(bike.ID);
        }

        internal void SendChat(int bikeId, string message)
        {
            foreach (var bike in bikes)
                if (bike.BikeId == bikeId)
                    sc.Chat(bike.ID, message);
        }

        internal void BroadCast(string message)
        {
            sc.Broadcast(message);

            // Add message to all active clients.
            foreach (var bike in bikes)
            {
                var newLabel = new Label();
                newLabel.Content = message;
                ((StackPanel) patientUserControl.FindName("ChatView" + bike.BikeId)).Children.Add(newLabel);
            }
        }

        internal void EmergencyStop()
        {
            sc.EmergencyStopSessions();
        }

        internal void UpdateResistance(int bikeId, string resistance)
        {
            foreach (var bike in bikes)
                if (bike.BikeId == bikeId)
                {
                    SetNewGuiLabelValue(bikeId, resistance, SetHomeResistanceCB);
                    sc.SetNewResistance(bike.ID, resistance);
                }
        }

        #region // GUI Callbacks

        // Home screen
        private void SetHomeClientNameCB(int bikeId, string name)
        {
            ((Label) homeUserControl.FindName("ClientBox" + bikeId)).Content = name;
        }

        private void SetHomeHeartRateCB(int bikeId, string heartrate)
        {
            ((Label) homeUserControl.FindName("HeartrateBox" + bikeId)).Content = heartrate + " BPM";
        }

        private void SetHomeSpeedCB(int bikeId, string speed)
        {
            ((Label) homeUserControl.FindName("SpeedBox" + bikeId)).Content = speed + " m/s";
        }

        private void SetHomeResistanceCB(int bikeId, string resistance)
        {
            ((Label) homeUserControl.FindName("ResistanceBox" + bikeId)).Content = resistance + "%";
        }

        // Patient screen.
        private void SetPatientClientNameCB(int bikeId, string name)
        {
            ((Label) patientUserControl.FindName("ClientLabel" + bikeId)).Content = name;
        }

        private void SetPatientHeartRateCB(int bikeId, string heartRate)
        {
            ((Label) patientUserControl.FindName("HeartrateLabel" + bikeId)).Content = heartRate + "BPM";
        }

        private void SetPatientAvgHeartRateCB(int bikeId, string avgHeartRate)
        {
            ((Label) patientUserControl.FindName("HeartrateAverageLabel" + bikeId)).Content = avgHeartRate + "BPM";
        }

        private void SetPatientSpeedCB(int bikeId, string speed)
        {
            ((Label) patientUserControl.FindName("SpeedLabel" + bikeId)).Content = speed + " m/s";
        }

        private void ClearChatBox(int bikeId, string msg)
        {
            ((StackPanel) patientUserControl.FindName("ChatView" + bikeId)).Children.Clear();
        }

        private void SetPatientAvgSpeedCB(int bikeId, string avgSpeed)
        {
            ((Label) patientUserControl.FindName("SpeedAverageLabel" + bikeId)).Content = avgSpeed + " m/s";
        }

        // Chart Callbacks
        public delegate void GuiChartCallBack(int bikeId);

        public void SetNewGuiChartLink(int bikeId, GuiChartCallBack callback)
        {
            Application.Current.Dispatcher.Invoke(callback, bikeId);
        }

        private void LinkHeartRateChartCB(int bikeId)
        {
            foreach (var bike in bikes)
                if (bike.BikeId == bikeId)
                {
                    ((Chart) patientUserControl.FindName("HeartRateChart" + bikeId)).Series[0].Values =
                        bike.HeartRateValues;
                    ((Chart) patientUserControl.FindName("HeartRateChart" + bikeId)).DataContext = bike;
                }
        }

        private void LinkSpeedChartCB(int bikeId)
        {
            foreach (var bike in bikes)
                if (bike.BikeId == bikeId)
                {
                    ((Chart) patientUserControl.FindName("SpeedChart" + bikeId)).Series[0].Values = bike.SpeedValues;
                    ((Chart) patientUserControl.FindName("SpeedChart" + bikeId)).DataContext = bike;
                }
        }

        // History screen.
        public delegate void GuiListCallBack(List<SessionData> records);

        public void SetNewGuiListValue(List<SessionData> records, GuiListCallBack callback)
        {
            Application.Current.Dispatcher.Invoke(callback, records);
        }

        /*
         * Fills the client box with clients from all the previous sessions.
         */
        private void SetHistoryClientBoxCB(List<SessionData> records)
        {
            var addedClients = new List<SessionData>();
            foreach (var rec in records)
            {
                var alreadyAdded = false;
                foreach (var client in addedClients)
                    if (client.ClientId == rec.ClientId)
                    {
                        alreadyAdded = true;
                        break;
                    }

                if (!alreadyAdded) addedClients.Add(rec);
            }

            var clients = new List<string>();
            foreach (var client in addedClients)
                clients.Add(client.Name + "\t\t" + client.ClientId);

            ((ListBox) historyUserControl.FindName("ClientListBox")).ItemsSource = clients;
            historyUserControl.Records = records;
        }

        #endregion
    }
}