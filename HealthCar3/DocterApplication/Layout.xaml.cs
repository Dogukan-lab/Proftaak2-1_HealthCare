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
        private HomeUserControl homeUserControl;
        private PatientUserControl patientUserControl;
        private HistoryUserControl historyUserControl;

        private string username;
        private ServerConnection sc;
        private List<Bike> bikes;

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
            historyUserControl = new HistoryUserControl();
            GridUserControls.Children.Add(historyUserControl);

            homeUserControl.Visibility = Visibility.Visible;
            patientUserControl.Visibility = Visibility.Hidden;
            historyUserControl.Visibility = Visibility.Hidden;
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
            
            homeUserControl.Visibility = Visibility.Hidden;
            historyUserControl.Visibility = Visibility.Hidden;

            patientUserControl.Visibility = Visibility.Visible;
           
        }

        private void HistoryButton_Click(object sender, RoutedEventArgs e)
        {
            homeUserControl.Visibility = Visibility.Hidden;
            patientUserControl.Visibility = Visibility.Hidden;

            historyUserControl.Visibility = Visibility.Visible;

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
            SetNewGuiLabelValue(bikes.Count, name, new GuiCallBack(SetHomeClientNameCB)); // Update client name
            SetNewGuiLabelValue(bikes.Count, "0", new GuiCallBack(SetHomeHeartRateCB)); // Set heart rate to 0
            SetNewGuiLabelValue(bikes.Count, "0", new GuiCallBack(SetHomeSpeedCB)); // Set speed to 0
            SetNewGuiLabelValue(bikes.Count, "0", new GuiCallBack(SetHomeResistanceCB)); // Set resistance to 0

            // Update patient screen labels
            SetNewGuiLabelValue(bikes.Count, name, new GuiCallBack(SetPatientClientNameCB)); // Update client name
            SetNewGuiLabelValue(bikes.Count, "0", new GuiCallBack(SetPatientHeartRateCB)); // Set heart rate to 0
            SetNewGuiLabelValue(bikes.Count, "0", new GuiCallBack(SetPatientAvgHeartRateCB)); // Set avg heart rate to 0
            SetNewGuiLabelValue(bikes.Count, "0", new GuiCallBack(SetPatientSpeedCB)); // Set speed to 0
            SetNewGuiLabelValue(bikes.Count, "0", new GuiCallBack(SetPatientAvgSpeedCB)); // Set avg speed to 0
        }

        internal void RemoveClient(string clientId)
        {
            foreach(var bike in bikes)
            {
                if(bike.ID == clientId)
                {
                    // Reset home screen labels
                    SetNewGuiLabelValue(bikes.Count, "-", new GuiCallBack(SetHomeClientNameCB)); // Remove client name
                    SetNewGuiLabelValue(bikes.Count, "0", new GuiCallBack(SetHomeHeartRateCB)); // Set heart rate to 0
                    SetNewGuiLabelValue(bikes.Count, "0", new GuiCallBack(SetHomeSpeedCB)); // Set speed to 0
                    SetNewGuiLabelValue(bikes.Count, "0", new GuiCallBack(SetHomeResistanceCB)); // Set resistance to 0

                    // Reset patient screen labels
                    SetNewGuiLabelValue(bikes.Count, "-", new GuiCallBack(SetPatientClientNameCB)); // Remove client name
                    SetNewGuiLabelValue(bikes.Count, "0", new GuiCallBack(SetPatientHeartRateCB)); // Set heart rate to 0
                    SetNewGuiLabelValue(bikes.Count, "0", new GuiCallBack(SetPatientAvgHeartRateCB)); // Set avg heart rate to 0
                    SetNewGuiLabelValue(bikes.Count, "0", new GuiCallBack(SetPatientSpeedCB)); // Set speed to 0
                    SetNewGuiLabelValue(bikes.Count, "0", new GuiCallBack(SetPatientAvgSpeedCB)); // Set avg speed to 0

                    bikes.Remove(bike);
                    return;
                }
            }   
        }

        public void NewHeartRate(string id, int newHeartRate)
        {
            foreach(var bike in bikes)
            {
                if(bike.ID == id)
                {
                    bike.NewHeartRate(newHeartRate);

                    // Update the GUI with the new values.
                    SetNewGuiLabelValue(bike.BikeId, newHeartRate.ToString(), new GuiCallBack(SetHomeHeartRateCB));
                    SetNewGuiLabelValue(bike.BikeId, bike.CurrentHeartRate.ToString(), new GuiCallBack(SetPatientHeartRateCB));
                    SetNewGuiLabelValue(bike.BikeId, bike.AverageHeartRate.ToString(), new GuiCallBack(SetPatientAvgHeartRateCB));
                }
            }
        }

        public void NewSpeed(string id, int newSpeed)
        {
            foreach (var bike in bikes)
            {
                if (bike.ID == id)
                {
                    bike.NewSpeed(newSpeed);

                    // Update the GUI with the new values.
                    SetNewGuiLabelValue(bike.BikeId, newSpeed.ToString(), new GuiCallBack(SetHomeSpeedCB));
                    SetNewGuiLabelValue(bike.BikeId, bike.CurrentSpeed.ToString(), new GuiCallBack(SetPatientSpeedCB));
                    SetNewGuiLabelValue(bike.BikeId, bike.AverageSpeed.ToString(), new GuiCallBack(SetPatientAvgSpeedCB));
                }
            }
        }

        public delegate void GuiCallBack(int bikeId, string newValue);

        public void SetNewGuiLabelValue(int bikeId, string newValue, GuiCallBack callback)
        {
            Application.Current.Dispatcher.Invoke(callback, new Object[] { bikeId, newValue });
        }

        #region // GUI Callbacks
        // Home screen
        private void SetHomeClientNameCB(int bikeId, string name)
        {
            ((Label)homeUserControl.FindName("ClientBox"+ bikeId)).Content = name;
        }
        private void SetHomeHeartRateCB(int bikeId, string heartrate)
        {
            ((Label)homeUserControl.FindName("HeartrateBox" + bikeId)).Content = heartrate + " BPM";
        }
        private void SetHomeSpeedCB(int bikeId, string speed)
        {
            ((Label)homeUserControl.FindName("SpeedBox" + bikeId)).Content = speed + " m/s";
        }
        private void SetHomeResistanceCB(int bikeId, string resistance)
        {
            ((Label)homeUserControl.FindName("ResistanceBox" + bikeId)).Content = resistance + "%";
        }

        // Patient screen.
        private void SetPatientClientNameCB(int bikeId, string name)
        {
            ((Label)patientUserControl.FindName("ClientLabel" + bikeId)).Content = name;
        }
        private void SetPatientHeartRateCB(int bikeId, string heartRate)
        {
            ((Label)patientUserControl.FindName("HeartrateLabel" + bikeId)).Content = heartRate + "BPM";
        }
        private void SetPatientAvgHeartRateCB(int bikeId, string avgHeartRate)
        {
            ((Label)patientUserControl.FindName("HeartrateAverageLabel" + bikeId)).Content = avgHeartRate + "BPM";
        }
        private void SetPatientSpeedCB(int bikeId, string speed)
        {
            ((Label)patientUserControl.FindName("SpeedLabel" + bikeId)).Content = speed + " m/s";
        }
        private void SetPatientAvgSpeedCB(int bikeId, string avgSpeed)
        {
            ((Label)patientUserControl.FindName("SpeedAverageLabel" + bikeId)).Content = avgSpeed + " m/s";
        }
        #endregion

        internal void StartSession(int bikeId)
        {
            foreach(var bike in bikes)
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
        }

        internal void EmergencyStop()
        {
            sc.EmergencyStopSessions();
        }
    }
}
